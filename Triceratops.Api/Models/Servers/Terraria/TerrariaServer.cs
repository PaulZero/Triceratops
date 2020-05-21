using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Models.Servers.Terraria
{
    public class TerrariaServer : AbstractWrappedServer<TerrariaConfiguration>
    {
        protected const string DockerImageName = "triceratops_terraria";

        protected const string DockerImageTag = "1.3.5.3";

        public TerrariaServer(Server serverEntity) : base(serverEntity)
        {
        }

        public static async Task<TerrariaServer> CreateAsync(TerrariaConfiguration configuration, IServerService serverService)
        {
            var server = serverService
                .GetServerBuilder(configuration)
                .CreateContainers(b =>
                {
                    b.CreateContainer(DockerImageName, DockerImageTag, "Terraria")
                     .BindPorts(configuration.HostPort, configuration.ContainerPort)
                     .CreateVolume("world", "/world")
                     .CreateVolume("tshock-plugins", "/tshock/ServerPlugins");
                })
                .GetServer();

            await serverService.CreateServerAsync(server);

            return new TerrariaServer(server);
        }
    }
}
