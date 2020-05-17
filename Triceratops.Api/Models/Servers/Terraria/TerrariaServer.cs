using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Models.Servers.Terraria
{
    public class TerrariaServer : AbstractWrappedServer<TerrariaConfiguration>
    {
        protected const string DockerImageName = "triceratops_terraria";

        protected const string DockerImageTag = "1.0";

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
                Name = $"Terraria_{server.Slug}",
                ImageName = DockerImageName,
                ImageVersion = DockerImageTag,
                ServerPorts = new[]
                {
                    new ServerPorts
                    {
                        ContainerPort = configuration.ContainerPort,
                        HostPort = configuration.HostPort
                    }
                },
                Volumes = new[]
                {
                    new Volume
                    {
                        Name = $"Terraria_{server.Slug}_world",
                        ContainerMountPoint = "/world"
                    },
                    new Volume
                    {
                        Name = $"Terraria_{server.Slug}_tshock_plugins",
                        ContainerMountPoint = "/tshock/ServerPlugins"
                    }
                }
            });

            await serverService.CreateServerAsync(server);

            return new TerrariaServer(server);
        }
    }
}
