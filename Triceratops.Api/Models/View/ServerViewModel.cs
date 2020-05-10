using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Servers;
using Triceratops.Api.Models.Servers.Minecraft;

namespace Triceratops.Api.Models.View
{
    public class ServerViewModel
    {
        public string Name => _server.Name;

        public string Game => Enum.GetName(typeof(ServerType), GetServerType());

        public ContainerViewModel[] Containers => _containers.ToArray();

        private readonly List<ContainerViewModel> _containers = new List<ContainerViewModel>();

        private readonly Server _server;

        public ServerViewModel(Server server)
        {
            _server = server;
        }

        public void AddContainer(ContainerViewModel container)
        {
            _containers.Add(container);
        }

        public ServerType GetServerType()
        {
            if (_server.ConfigurationType == typeof(MinecraftConfiguration))
            {
                return ServerType.Minecraft;
            }

            return ServerType.Unknown;
        }
    }
}
