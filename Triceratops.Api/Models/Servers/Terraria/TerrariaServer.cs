using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Models.Servers.Terraria
{
    public class TerrariaServer : AbstractWrappedServer<TerrariaConfiguration>
    {
        protected const string DockerImageName = "ryshe/terraria";

        protected const string DockerImageTag = "latest";

        public TerrariaServer(Server serverEntity) : base(serverEntity)
        {
        }

        public static async Task<TerrariaServer> CreateAsync(TerrariaConfiguration configuration, IServerService serverService)
        {
            if (!configuration.IsValid())
            {
                throw new Exception("Cannot create a new Terraria server from an invalid config");
            }

            var server = new Server
            {
                Name = $"{configuration.ServerName.Replace(' ', '_')}"
            };

            server.SetConfiguration(configuration);

            server.Containers.Add(new Container
            {
                Name = configuration.ServerName,
                ImageName = DockerImageName,
                ImageVersion = DockerImageTag,
                ServerPorts = new[]
                {
                    new ServerPorts
                    {
                        ContainerPort = configuration.ContainerPort,
                        HostPort = configuration.HostPort
                    }
                }
            });

            await serverService.CreateServerAsync(server);

            return new TerrariaServer(server);
        }
    }
}
