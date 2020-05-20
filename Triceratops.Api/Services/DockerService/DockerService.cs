using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DockerService
{
    public class DockerService : IDockerService
    {
        private const string ContainerNamePrefix = "TRICERATOPS_";

        private const string VolumeContainerName = "Triceratops.VolumeManager";

        private readonly ILogger _logger;

        public DockerService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> CreateContainerAsync(Container container)
        {
            try
            {
                using var dockerClient = CreateDockerClient();

                if (container.ImageName.StartsWith("triceratops"))
                {
                    await BuildImageAsync(container.ImageName, container.ImageVersion, dockerClient);
                }
                else
                {
                    await DownloadImageAsync(container.ImageName, container.ImageVersion, dockerClient);
                }

                var createParameters = CreateContainerParameters(container);

                var response = await dockerClient.Containers.CreateContainerAsync(createParameters);

                if (response.Warnings.Any())
                {
                    // Do some stuff with the warnings.

                    Debug.WriteLine($"{response.Warnings.Count} warnings were generated");
                }

                container.DockerId = response.ID;

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to create new container: {exception.Message}");
                
                return false;
            }
        }

        public async Task<ContainerDetails> GetContainerStatusAsync(Container container)
        {
            try
            {
                using var dockerClient = CreateDockerClient();
                var response = await dockerClient.Containers.InspectContainerAsync(container.DockerId);

                return new ContainerDetails(container.DockerId, response.State.Status, response.Created, GetTriceratopsContainerState(response.State));
            }
            catch (DockerContainerNotFoundException)
            {
                return ContainerDetails.ContainerMissing;
            }
            catch
            {
                return ContainerDetails.ErrorInspectingContainer;
            }
        }

        public async Task StopContainerAsync(string containerId)
        {
            try
            {
                using var dockerClient = CreateDockerClient();

                await dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to stop container: {exception.Message}");
            }
        }

        public async Task DeleteContainerAsync(string containerId, bool force = false)
        {
            try
            {
                using var dockerClient = CreateDockerClient();

                await dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
                {
                    Force = force
                });

                // TODO: Clean up the files in gamedata!
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to delete container: {exception.Message}");
            }
        }

        public async Task RunContainerAsync(string containerId, params string[] parameters)
        {
            try
            {
                using var dockerClient = CreateDockerClient();

                await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to run container: {exception.Message}");
            }
        }

        public async Task UpdateVolumeServerAsync(Server[] servers)
        {
            using var dockerClient = CreateDockerClient();

            await DeleteVolumeServer(dockerClient);

            var volumes = new List<Volume>();

            foreach (var server in servers)
            {
                if (!server.HasVolumes)
                {
                    continue;
                }

                var serverName = server.Slug;

                foreach (var container in server.Containers)
                {
                    foreach (var volume in container.Volumes)
                    {
                        volumes.Add(new Volume
                        {
                            DockerName = volume.DockerName,
                            ContainerMountPoint = $"/app/volumes/{serverName}/{volume.DisplayName}"
                        });
                    }
                }
            }

            var volumeContainer = new Container
            {
                Name = VolumeContainerName,
                ImageName = "triceratops_volumemanager",
                ImageVersion = "latest",
                Volumes = volumes,
                ServerPorts = new List<ServerPorts>
                {
                    new ServerPorts
                    {
                        HostPort = 7070,
                        ContainerPort = 80
                    }
                }
            };

            var creationParameters = CreateContainerParameters(volumeContainer, false);

            creationParameters.Hostname = VolumeContainerName.ToLower();

            await DownloadImageAsync(volumeContainer.ImageName, volumeContainer.ImageVersion, dockerClient);

            var response = await dockerClient.Containers.CreateContainerAsync(creationParameters);

            await dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
        }

        public async Task<string[]> GetContainerLogAsync(string containerId, uint rows = 300)
        {
            if (rows > 300)
            {
                rows = 300;
            }

            if (rows < 10)
            {
                rows = 10;
            }

            using var dockerClient = CreateDockerClient();
            using var stream = await dockerClient.Containers.GetContainerLogsAsync(containerId, new ContainerLogsParameters
            {
                Tail = rows.ToString(),
                ShowStdout = true
            });
            using var reader = new StreamReader(stream);

            var lines = new List<string>();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                var lineBytes = Encoding.ASCII.GetBytes(line);

                if (lineBytes.Length <= 8)
                {
                    lines.Add("");

                    continue;
                }

                line = Encoding.ASCII.GetString(lineBytes.Skip(8).ToArray());

                lines.Add(line);
            }

            return lines.ToArray();
        }

        private async Task DeleteVolumeServer(DockerClient dockerClient)
        {
            // TODO: Don't just search the entire bloody list...

            var allContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            foreach (var container in allContainers)
            {
                if (container.Names.Contains($"/{VolumeContainerName}"))
                {
                    await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                    {
                        Force = true,
                        RemoveVolumes = false
                    });
                }
            }

            await dockerClient.Volumes.PruneAsync();
        }

        private async Task BuildDockerImage(FileInfo dockerFile, ImageConfig imageConfig, ImageVersion imageVersion, DockerClient dockerClient)
        {
            using var tarball = new MemoryStream();
            using var archive = new TarOutputStream(tarball)
            {
                IsStreamOwner = false
            };

            var entry = TarEntry.CreateTarEntry(dockerFile.Name);
            var fileStream = File.OpenRead(dockerFile.FullName);
            entry.Size = fileStream.Length;
            archive.PutNextEntry(entry);

            await fileStream.CopyToAsync(archive);

            archive.CloseEntry();
            archive.Close();

            tarball.Position = 0;

            await dockerClient.Images.BuildImageFromDockerfileAsync(tarball, new ImageBuildParameters
            {
                Tags = new List<string>
                {
                    $"{imageConfig.Name}:{imageVersion.Tag}"
                },
                BuildArgs = imageVersion.EnvironmentVariables
            });
        } 

        private async Task BuildImageAsync(string imageName, string imageTag, DockerClient dockerClient)
        {
            if (await IsImageRealAsync($"{imageName}:{imageTag}", dockerClient))
            {
                return;
            }

            var dockerDirectories = Directory.GetDirectories("Dockerfiles");
            var imageIdentifier = $"{imageName}:{imageTag}";

            foreach (var directory in dockerDirectories)
            {
                var files = Directory.GetFiles(directory).Select(f => new FileInfo(f));
                var imageConfigFile = files.FirstOrDefault(f => f.Name == "ImageConfig.json");
                var dockerFile = files.FirstOrDefault(f => f.Name == "Dockerfile");

                if (imageConfigFile == null || dockerFile == null)
                {
                    continue;
                }

                var imageConfig = JsonConvert.DeserializeObject<ImageConfig>(File.ReadAllText(imageConfigFile.FullName));

                if (imageConfig.Name == imageName)
                {
                    var version = imageConfig.Versions.FirstOrDefault(v => v.Tag == imageTag)
                        ?? throw new Exception($"The tag {imageTag} was not found for image {imageName}.");

                    await BuildDockerImage(dockerFile, imageConfig, version, dockerClient);

                    await WaitForDockerImageBuildAsync($"{imageConfig.Name}:{version.Tag}", dockerClient);

                    return;
                }
            }

            throw new Exception("No image could be found for the requested server.");
        }

        private Task WaitForDockerImageBuildAsync(string imageIdentifier, DockerClient dockerClient)
            => RetryHelper.RetryTask(() => IsImageRealAsync(imageIdentifier, dockerClient));            

        private async Task<bool> IsImageRealAsync(string imageIdentifier, DockerClient dockerClient)
        {
            try
            {
                var response = await dockerClient.Images.InspectImageAsync(imageIdentifier);

                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private CreateContainerParameters CreateContainerParameters(Container container, bool prefixContainer = true)
        {
            var exposedPorts = new Dictionary<string, EmptyStruct>();
            var portBindings = new Dictionary<string, IList<PortBinding>>();

            foreach (var portBinding in container.ServerPorts)
            {
                var containerPort = portBinding.ContainerPort.ToString();
                var hostPort = portBinding.HostPort.ToString();

                exposedPorts.Add(containerPort, new EmptyStruct());
                portBindings.Add(containerPort, new List<PortBinding>
                {
                    new PortBinding
                    {
                        HostIP = "0.0.0.0",
                        HostPort = hostPort
                    }
                });
            }

            var parameters = new CreateContainerParameters
            {
                Image = $"{container.ImageName}:{container.ImageVersion}",
                Name = prefixContainer ? $"{ContainerNamePrefix}{container.Name}" : container.Name,
                Env = container.Arguments?.ToList(),
                ExposedPorts = exposedPorts,
                HostConfig = new HostConfig
                {
                    PortBindings = portBindings,
                    Mounts = container.Volumes.Select(v =>
                    {
                        return new Mount
                        {
                            Source = v.DockerName,
                            Target = v.ContainerMountPoint,
                            Type = "volume"
                        };
                    }).ToList(),
                    NetworkMode = "triceratops.network"
                }
            };

            return parameters;
        }

        private async Task DownloadImageAsync(string imageName, string version, DockerClient dockerClient)
        {
            try
            {
                if (imageName.StartsWith("triceratops_"))
                {
                    return;
                }

                await dockerClient.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = imageName,
                        Tag = version,
                    },
                    null,
                    new Progress<JSONMessage>(m => Debug.WriteLine(m.ProgressMessage))
                );
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Failed to download image: {exception.Message}");
            }
        }

        private ServerContainerState GetTriceratopsContainerState(ContainerState state)
        {
            switch (state.Status)
            {
                case "running":
                    return ServerContainerState.Running;

                case "created":
                    return ServerContainerState.Created;

                case "paused":
                    return ServerContainerState.Paused;

                case "exited":
                    return ServerContainerState.Stopped;

                default:
                    throw new Exception($"Unknown container state: {state.Status}");
            }
        }

        private static DockerClient CreateDockerClient()
        {
            

            var apiUri = Environment.GetEnvironmentVariable("DOCKER_API_URI") ?? "tcp://host.docker.internal:2375";

            

            return new DockerClientConfiguration(new Uri(apiUri))
                .CreateClient();
        }
    }
}
