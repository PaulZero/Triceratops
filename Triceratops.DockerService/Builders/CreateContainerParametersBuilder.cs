using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Triceratops.DockerService.Builders.Interfaces;
using Triceratops.DockerService.Helpers;
using Triceratops.DockerService.Structs;
using Triceratops.Libraries;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.DockerService.Builders
{
    public class CreateContainerParametersBuilder : IDockerParameterBuilder<CreateContainerParameters>
    {
        private const string MountTypeVolume = "volume";

        public DockerImageId ImageId { get; }

        protected string ContainerName { get; }

        protected IList<string> EnvironmentVariables { get; }
            = new List<string>();

        protected IDictionary<string, EmptyStruct> ExposedPorts
            => PortBindings.ToDictionary(kvp => kvp.Key, kvp => new EmptyStruct());

        protected IDictionary<string, string> Labels { get; }
            = new Dictionary<string, string>();

        protected string NetworkName { get; }

        protected IDictionary<string, IList<PortBinding>> PortBindings { get; }
            = new Dictionary<string, IList<PortBinding>>();

        protected IList<Mount> Mounts { get; }
            = new List<Mount>();

        public CreateContainerParametersBuilder(DockerImageId imageId, string containerName)
        {
            ImageId = imageId;
            ContainerName = containerName;
            NetworkName = DockerServiceConstants.TriceratopsNetwork;
        }

        public CreateContainerParametersBuilder AddEnvironmentVariable(string name, string value)
        {
            return AddEnvironmentVariable($"{name}={value}");
        }

        public CreateContainerParametersBuilder AddEnvironmentVariable(string variable)
        {
            EnvironmentVariables.Add(variable);

            return this;
        }

        public CreateContainerParametersBuilder AddLabel(string name, string value)
        {
            Labels.Add(name, value);

            return this;
        }

        public CreateContainerParametersBuilder AddLabels(IDictionary<string, string> labels)
        {
            foreach (var (name, value) in labels)
            {
                AddLabel(name, value);
            }

            return this;
        }

        public CreateContainerParametersBuilder AddPortBinding(ushort containerPort, ushort hostPort)
        {
            return AddPortBinding(containerPort.ToString(), hostPort.ToString());
        }

        public CreateContainerParametersBuilder AddVolumeMount(string volumeName, string containerMountPath, IDictionary<string, string> labels, bool isReadOnly = false)
        {
            Mounts.Add(new Mount
            {
                Source = volumeName,
                Target = containerMountPath,
                Type = MountTypeVolume,
                ReadOnly = isReadOnly,
                VolumeOptions = new VolumeOptions
                {
                    Labels = labels
                }
            });

            return this;
        }

        public CreateContainerParameters CreateParameters()
        {
            return new CreateContainerParameters
            {
                Image = ImageId.FullName,
                Name = ContainerName,
                Env = EnvironmentVariables,
                ExposedPorts = ExposedPorts,
                HostConfig = new HostConfig
                {
                    PortBindings = PortBindings,
                    Mounts = Mounts,
                    NetworkMode = NetworkName,
                },
                Labels = Labels
            };
        }

        public void LogParameters(ILogger logger)
        {
            using var contentsScope = logger.BeginScope("Using create container parameters");

            logger.LogInformation($"Container name '{ContainerName}'");
            logger.LogInformation($"Container image: '{ImageId.FullName}");
            logger.LogInformation($"Network name: {ImageId.FullName}");

            using (logger.BeginScope("Labels"))
            {
                if (!Labels.Any())
                {
                    logger.LogError("No labels are configured for this container!");
                }

                foreach (var (name, value) in Labels)
                {
                    logger.LogInformation($"{name}: {value}");
                }
            }

            using (logger.BeginScope("Environment variables"))
            {
                if (!EnvironmentVariables.Any())
                {
                    logger.LogInformation("No environment variables are set");
                }

                foreach (var variable in EnvironmentVariables)
                {
                    logger.LogInformation(variable);
                }
            }

            using (logger.BeginScope("Volume mounts"))
            {
                if (!Mounts.Any())
                {
                    logger.LogWarning("There are no volume mounts specified for this container");
                }

                foreach (var mount in Mounts)
                {
                    logger.LogInformation($"Volume '{mount.Source}' mounted to '{mount.Target}'");
                }
            }

            using (logger.BeginScope("Port bindings"))
            {
                if (!PortBindings.Any())
                {
                    logger.LogWarning("There are no port bindings specified for this container");
                }

                foreach (var (containerPort, hostBindings) in PortBindings)
                {
                    foreach (var binding in hostBindings)
                    {
                        logger.LogInformation(
                            $"Container port {containerPort} is bound to {binding.HostIP}:{binding.HostPort}"
                        );
                    }
                }
            }
        }

        public static CreateContainerParametersBuilder CreateFromContainer(Container container)
        {
            var imageId = new DockerImageId(container.ImageName, container.ImageVersion);
            var builder = new CreateContainerParametersBuilder(imageId, container.Name);

            foreach (var (name, value) in DockerLabelHelper.CreateForContainer(container))
            {
                builder.AddLabel(name, value);
            }

            foreach (var binding in container.ServerPorts)
            {
                builder.AddPortBinding(binding.ContainerPort, binding.HostPort);
            }

            foreach (var volume in container.Volumes)
            {
                builder.AddVolumeMount(
                    volume.DockerName,
                    volume.ContainerMountPoint,
                    DockerLabelHelper.CreateForVolume(container, volume)
                );
            }

            foreach (var argument in container.Arguments)
            {
                builder.AddEnvironmentVariable(argument);
            }

            return builder;
        }

        public static CreateContainerParametersBuilder CreateStorageServer(Server server)
        {
            var imageId = new DockerImageId("triceratops_volumeinspector", "1.0");
            var containerName = NameHelper.SanitiseHostname($"Triceratops.StorageServer.{server.Name}");

            var builder = new CreateContainerParametersBuilder(imageId, containerName)
                .AddLabels(DockerLabelHelper.CreateForTemporaryContainer());

            foreach (var volume in ServerVolumeIdentifier.CreateForServer(server))
            {
                builder.AddVolumeMount(
                    volume.VolumeName,
                    volume.CreateMountDestination(Constants.VolumeInspectorVolumesPath),
                    new Dictionary<string, string>(0)
                );
            };

            return builder;
        }

        private CreateContainerParametersBuilder AddPortBinding(string containerPort, string hostPort)
        {
            if (!PortBindings.ContainsKey(containerPort))
            {
                PortBindings.Add(containerPort, new List<PortBinding>());
            }

            PortBindings[containerPort].Add(new PortBinding
            {
                HostIP = "0.0.0.0",
                HostPort = hostPort
            });

            return this;
        }
    }
}
