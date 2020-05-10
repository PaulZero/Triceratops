using System;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;

namespace Triceratops.Api.Models.Servers.Minecraft
{
    public class MinecraftServer : AbstractWrappedServer<MinecraftConfiguration>
    {
        protected const string DockerImageName = "itzg/minecraft-server";

        protected const string DockerImageTag = "latest";

        public override ServerType ServerType => ServerType.Minecraft;

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
                Name = $"Minecraft-{configuration.ServerName.Replace(' ', '_')}"
            };

            server.SetConfiguration(configuration);

            server.Containers.Add(new Container
            {
                Name = server.Name,
                ImageName = DockerImageName,
                ImageVersion = DockerImageTag,
                Port = configuration.Port,
                Arguments = new string[]
                {
                    "EULA=TRUE"
                }
            });

            await serverService.CreateServerAsync(server);

            return new MinecraftServer(server);
        }
    }
}
