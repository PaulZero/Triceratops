using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Servers;
using Triceratops.Api.Models.View.Transformers.Interfaces;
using Triceratops.Api.Services.DockerService;
using Triceratops.Libraries.Models.View;

namespace Triceratops.Api.Models.View.Transformers
{
    public class ViewModelTransformer : IViewModelTransformer
    {
        private readonly IDockerService _dockerService;

        private readonly ServerHelper _serverHelper = new ServerHelper();

        public ViewModelTransformer(IDockerService dockerService)
        {
            _dockerService = dockerService;
        }

        public async Task<ContainerViewModel> WrapContainerAsync(Container container)
        {
            var details = await _dockerService.GetContainerStatusAsync(container);

            return new ContainerViewModel
            {
                Name = container.Name,
                DockerId = container.DockerId,
                Status = details.Status,
                Created = details.Created,
                IsRunning = details.IsRunning
            };
        }

        public async Task<List<ContainerViewModel>> WrapContainersAsync(IEnumerable<Container> containers)
        {
            return (await Task.WhenAll(containers.Select(c => WrapContainerAsync(c)))).ToList();
        }

        public async Task<ServerViewModel> WrapServerAsync(Server server)
        {
            var containers = await WrapContainersAsync(server.Containers);

            return new ServerViewModel
            {
                Name = server.Name,
                ServerId = server.Id.ToString(),
                ServerType = _serverHelper.GetServerTypeForConfigurationType(server.ConfigurationType),
                Containers = containers
            };
        }

        public async Task<List<ServerViewModel>> WrapServersAsync(IEnumerable<Server> servers)
        {
            return (await Task.WhenAll(servers.Select(s => WrapServerAsync(s)))).ToList();
        }
    }
}
