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
            var server = serverService
                .GetServerBuilder(configuration)
                .CreateContainers(b =>
                {
                    b.CreateContainer(DockerImageName, DockerImageTag, "Minecraft")
                     .BindPorts(configuration.HostPort, configuration.ContainerPort)
                     .BindPorts(configuration.RconHostPort, configuration.RconContainerPort)
                     .CreateVolume("data", "/data")
                     .WithArguments("EULA=TRUE");
                })
                .GetServer();

            await serverService.CreateServerAsync(server);

            return new MinecraftServer(server);
        }
    }
}
