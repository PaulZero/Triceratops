using System;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;

namespace Triceratops.Api.Models.Servers.Minecraft
{
    public class MinecraftServer : AbstractWrappedServer<MinecraftConfiguration>
    {
        protected const string DockerImageName = "itzg/minecraft-server";

        protected const string DockerImageTag = "latest";

        public MinecraftServer(Server server) : base(server)
        {
        }

        public static async Task<MinecraftServer> CreateAsync(MinecraftConfiguration configuration, IServerService serverService)
        {
            if (!configuration.IsValid())
            {
                throw new Exception("Cannot create a new Minecraft server from an invalid config");
            }

            var server = new Server
            {
                Name = $"{configuration.ServerName.Replace(' ', '_')}"
            };

            server.SetConfiguration(configuration);

            server.Containers.Add(new Container
            {
                Name = server.Name,
                ImageName = DockerImageName,
                ImageVersion = DockerImageTag,
                ServerPorts = new[]
                {
                    new ServerPorts
                    {
                        ContainerPort = configuration.ContainerPort,   
                        HostPort = configuration.HostPort
                    },
                    new ServerPorts
                    {
                        ContainerPort = configuration.RconContainerPort,
                        HostPort = configuration.RconHostPort,
                    }
                },
                Arguments = new[]
                {
                    "EULA=TRUE",
                    "ENABLE_RCON=true",
                    "RCON_PASSWORD=testing",
                    $"RCON_PORT={configuration.RconContainerPort}",
                }
            });

            await serverService.CreateServerAsync(server);

            return new MinecraftServer(server);
        }
    }
}
