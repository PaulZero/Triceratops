using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Storage;
using Triceratops.VolumeInspector;

namespace Triceratops.Api.Services.DockerService
{
    public class DockerService : IDockerService, IDisposable
    {
        private const string ContainerNamePrefix = "TRICERATOPS_";

        private readonly string _dockerDaemonUrl;

        private readonly ILogger _logger;

        private readonly TemporaryContainerManager _temporaryContainers;

        public DockerService(string dockerDaemonUrl, ILogger logger)
        {
            _dockerDaemonUrl = dockerDaemonUrl;
            _logger = logger;
            _temporaryContainers = new TemporaryContainerManager(CreateDockerClient, TimeSpan.FromMinutes(1), _logger);
        }

        public async Task PrepareAsync()
        {
            using var dockerClient = CreateDockerClient();

#if (DEBUG)
            // Uncomment if you make changes to the volume inspector and need it to rebuild on start

            //try
            //{
            //    await dockerClient.Images.DeleteImageAsync("triceratops_volumeinspector:1.0", new ImageDeleteParameters
            //    {
            //        Force = true,
            //        PruneChildren = true
            //    });
            //}
            //catch
            //{
            //    // Lalalalalala...
            //}
#endif
            var existingStorageContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    ["label"] = new Dictionary<string, bool>
                    {
                        ["com.paulzero.triceratops.volumeinspector"] = true
                    }
                }
            });

            foreach (var container in existingStorageContainers)
            {
                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                {
                    Force = true,
                    RemoveVolumes = false
                });
            }

            await FindAndBuildDockerImageAsync("triceratops_volumeinspector", "1.0", dockerClient);
        }

        public async Task<TemporaryStorageContainer> GetStorageContainerAsync(Server server)
        {
            var existingContainer = _temporaryContainers.GetContainer(server.Id, true);

            if (existingContainer != null)
            {
                return existingContainer as TemporaryStorageContainer;
            }

            var dockerClient = CreateDockerClient();
            var serverVolumes = ServerVolumeIdentifier.CreateForServer(server);
            var mounts = serverVolumes.Select(v =>
            {
                return new Mount
                {
                    Source = v.VolumeName,
                    Target = v.CreateMountDestination(VolumeInspectorConstants.VolumesPath),
                    Type = "volume"
                };
            });

            var containerName = NameHelper.SanitiseHostname($"Triceratops.StorageServer.{server.Name}");

            var parameters = new CreateContainerParameters
            {
                Name = containerName,
                Image = "triceratops_volumeinspector:1.0",
                Labels = new Dictionary<string, string>
                {
                    ["com.paulzero.triceratops.volumeinspector"] = "yes"
                },
                HostConfig = new HostConfig
                {
                    Mounts = mounts.ToList(),
                    NetworkMode = "triceratops.network",
                }
            };

            var response = await dockerClient.Containers.CreateContainerAsync(parameters);
            var containerId = response.ID;

            await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

            var httpClient = CoreHttpClient.Create(_logger, $"http://{containerName}");

            await RetryHelper.RetryTask(retries => httpClient.CheckUrlReturnsOkAsync("/verify"));

            var container = new TemporaryStorageContainer(httpClient, server.Id, containerId);

            _temporaryContainers.AddContainer(container);

            return container;
        }

        public async Task<bool> CreateContainerAsync(Container container)
        {
            try
            {
                using var dockerClient = CreateDockerClient();

                if (container.ImageName.StartsWith("triceratops"))
                {
                    await FindAndBuildDockerImageAsync(container.ImageName, container.ImageVersion, dockerClient);
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

        private async Task BuildDockerImageAsync(DirectoryInfo directory, ImageConfig imageConfig, ImageVersion imageVersion, DockerClient dockerClient)
        {
            try
            {
                var tarBuilder = new TarBuilder();
                using var tarball = await tarBuilder.BuildFromDirectory(directory);

                using var writeStream = File.OpenWrite("/app/testcrap/taaaaaar.tar");

                await tarball.CopyToAsync(writeStream);

                tarball.Position = 0;

                var parameters = new ImageBuildParameters
                {
                    Tags = new List<string>
                    {
                        $"{imageConfig.Name}:{imageVersion.Tag}"
                    }
                };

                if (imageVersion.HasEnvironmentVariables)
                {
                    parameters.BuildArgs = imageVersion.EnvironmentVariables;
                }

                using var stream = await dockerClient.Images.BuildImageFromDockerfileAsync(tarball, parameters);
                using var reader = new StreamReader(stream);

                var content = reader.ReadToEnd();
            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to build image: {exception.Message}");

                throw;
            }
        }

        private async Task FindAndBuildDockerImageAsync(string imageName, string imageTag, DockerClient dockerClient)
        {
            if (await IsImageRealAsync($"{imageName}:{imageTag}", dockerClient))
            {
                return;
            }

            var dockerDirectories = Directory.GetDirectories("/app/dockersources/");
            var imageIdentifier = $"{imageName}:{imageTag}";

            foreach (var directory in dockerDirectories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var files = directoryInfo.GetFiles();
                var imageConfigFile = files.FirstOrDefault(f => f.Name == "ImageConfig.json");
                var dockerFile = files.FirstOrDefault(f => f.Name == "Dockerfile");

                if (imageConfigFile == null || dockerFile == null)
                {
                    continue;
                }

                var imageConfig = JsonHelper.Deserialise<ImageConfig>(File.ReadAllText(imageConfigFile.FullName));

                if (imageConfig.Name == imageName)
                {
                    var version = imageConfig.Versions.FirstOrDefault(v => v.Tag == imageTag)
                        ?? throw new Exception($"The tag {imageTag} was not found for image {imageName}.");

                    await BuildDockerImageAsync(directoryInfo, imageConfig, version, dockerClient);

                    await WaitForDockerImageBuildAsync($"{imageConfig.Name}:{version.Tag}", dockerClient);

                    return;
                }
            }

            throw new Exception("No image could be found for the requested server.");
        }

        private async Task WaitForDockerImageBuildAsync(string imageIdentifier, DockerClient dockerClient)
        {
            await RetryHelper.RetryTask(async retry =>
            {
                if (await IsImageRealAsync(imageIdentifier, dockerClient))
                {
                    _logger.LogWarning($"Failed checking to see if {imageIdentifier} exists (retries: {retry}).");

                    return false;
                }

                return true;
            }, 20, TimeSpan.FromSeconds(1));
        }          

        private async Task<bool> IsImageRealAsync(string imageIdentifier, DockerClient dockerClient)
        {
            try
            {
                var response = await dockerClient.Images.InspectImageAsync(imageIdentifier);

                return true;
            }
            catch
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
                    NetworkMode = "triceratops.network",
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

        private DockerClient CreateDockerClient()
        {
            return new DockerClientConfiguration(new Uri(_dockerDaemonUrl))
                .CreateClient();
        }

        public void Dispose()
        {
            _temporaryContainers.Dispose();
        }
    }
}
