using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DockerService.Models;

namespace Triceratops.Api.Services.DockerService
{
    public class DockerService : IDockerService
    {
        private const string ContainerNamePrefix = "TRICERATOPS_";

        public DockerService()
        {
            Task.WaitAll(Prepare());
        }

        public async Task Prepare()
        {
            Directory.CreateDirectory("Dockerfiles");

            var dockerDirectories = Directory.GetDirectories("Dockerfiles");

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

                await BuildDockerImage(dockerFile, imageConfig);
            }
        }

        private async Task BuildDockerImage(FileInfo dockerFile, ImageConfig imageConfig)
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

            using var dockerClient = CreateDockerClient();

            await dockerClient.Images.BuildImageFromDockerfileAsync(tarball, new ImageBuildParameters
            {
                Tags = new List<string>
                {
                    imageConfig.Tag
                }
            });
        }

        public async Task<bool> CreateContainerAsync(Container container, List<string> commands = null)
        {
            try
            {
                var id = await CreateContainerAsync(
                    container.ImageName,
                    container.ImageVersion,
                    $"{ContainerNamePrefix}{container.Name}",
                    container.ServerPorts,
                    container.Arguments
                );

                container.DockerId = id;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ContainerDetails> GetContainerStatusAsync(Container container)
        {
            try
            {
                using var dockerClient = CreateDockerClient();
                var response = await dockerClient.Containers.InspectContainerAsync(container.DockerId);

                return new ContainerDetails(response);
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

        private async Task<string> CreateContainerAsync(
            string imageName,
            string imageVersion,
            string containerName,
            IEnumerable<ServerPorts> serverPorts,
            IEnumerable<string> env = default
        )
        {
            await DownloadImageAsync(imageName, imageVersion);

            var exposedPorts = new Dictionary<string, EmptyStruct>();
            var portBindings = new Dictionary<string, IList<PortBinding>>();

            foreach (var portBinding in serverPorts)
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
            using var dockerClient = CreateDockerClient();

            var response = await dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                Image = $"{imageName}:{imageVersion}",
                Name = containerName,
                Env = env?.ToList(),
                ExposedPorts = exposedPorts,
                HostConfig = new HostConfig
                {
                    PortBindings = portBindings
                }
            });

            if (response.Warnings.Any())
            {
                // Do some stuff with the warnings.

                Debug.WriteLine($"{response.Warnings.Count} warnings were generated");
            }

            return response.ID;
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

        private async Task DownloadImageAsync(string imageName, string version)
        {
            try
            {
                if (imageName.StartsWith("triceratops_"))
                {
                    return;
                }

                using var dockerClient = CreateDockerClient();

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

        private static DockerClient CreateDockerClient()
        {
            var apiUri = Environment.GetEnvironmentVariable("DOCKER_API_URI") ?? "tcp://host.docker.internal:2375";

            return new DockerClientConfiguration(new Uri(apiUri))
                .CreateClient();
        }
    }
}
