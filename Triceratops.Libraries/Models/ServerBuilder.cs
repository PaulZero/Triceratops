using System;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Models
{
    public class ServerBuilder
    {
        protected AbstractServerConfiguration Configuration { get; private set; }

        protected Server Server { get; private set; }

        protected IServerValidator Validator { get; }

        public ServerBuilder(AbstractServerConfiguration configuration, IServerValidator validator)
        {
            if (!configuration.IsValid())
            {
                throw new Exception("Cannot create a server from an invalid configuration.");
            }

            Configuration = configuration;
            Server = new Server
            {
                Name = configuration.ServerName
            };
            Validator = validator;

            Server.SetConfiguration(Configuration);
        }

        public ServerBuilder CreateContainers(Action<ContainerListBuilder> builderCallback)
        {
            var builder = new ContainerListBuilder(this);

            builderCallback(builder);

            return this;
        }

        public Server GetServer()
        {
            Validator.ValidateServerAsync(Server);

            return Server;
        }

        public class ContainerListBuilder
        {
            protected ServerBuilder ServerBuilder { get; private set; }

            protected AbstractServerConfiguration Configuration => ServerBuilder.Configuration;

            protected Server Server => ServerBuilder.Server;

            public ContainerListBuilder(ServerBuilder serverBuilder)
            {
                ServerBuilder = serverBuilder;
            }

            public ContainerBuilder CreateContainer(string imageName, string imageTag = "latest", string customName = null)
            {
                return new ContainerBuilder(this, imageName, imageTag, customName);
            }

            public class ContainerBuilder
            {
                protected ContainerListBuilder ContainerListBuilder { get; private set; }

                protected Server Server => ContainerListBuilder.Server;

                protected AbstractServerConfiguration Configuration => ContainerListBuilder.Configuration;

                protected Container Container { get; }

                public ContainerBuilder(ContainerListBuilder containerListBuilder, string imageName, string imageTag = "latest", string customName = null)
                {
                    ContainerListBuilder = containerListBuilder;

                    Container = new Container
                    {
                        Name = NameHelper.CreateContainerName(Server, Configuration.ServerType, customName),
                        ServerId = Server.Id,
                        ImageName = imageName,
                        ImageVersion = imageTag
                    };

                    Server.Containers.Add(Container);
                }

                public ContainerBuilder BindPorts(ushort hostPort, ushort containerPort)
                {
                    Container.ServerPorts.Add(new ServerPorts
                    {
                        HostPort = hostPort,
                        ContainerPort = containerPort
                    });

                    return this;
                }

                public ContainerBuilder CreateVolume(string displayName, string mountPoint)
                {
                    Container.Volumes.Add(new Volume
                    {
                        ContainerId = Container.Id,
                        DisplayName = displayName,
                        DockerName = NameHelper.CreateVolumeName(Server, Configuration.ServerType, displayName),
                        ContainerMountPoint = mountPoint
                    });

                    return this;
                }

                public ContainerBuilder WithArguments(params string[] arguments)
                {
                    Container.Arguments.AddRange(arguments);

                    return this;
                }
            }
        }
    }
}
