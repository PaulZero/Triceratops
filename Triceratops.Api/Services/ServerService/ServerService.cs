using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Models.View;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Services.ServerService
{
    public class ServerService : IServerService
    {
        private IDbService DbService { get; }

        private IDockerService DockerService { get; }

        public ServerService(IDbService dbService, IDockerService dockerService)
        {
            DbService = dbService;
            DockerService = dockerService;
        }

        public async Task<ServerViewModel[]> GetServerViewListAsync()
        {
            var servers = await DbService.Servers.FindAllAsync();
            var serverViewModels = new List<ServerViewModel>();

            foreach (var server in servers)
            {
                var viewModel = new ServerViewModel(server);

                foreach (var container in server.Containers)
                {
                    viewModel.AddContainer(new ContainerViewModel(container, await DockerService.GetContainerStatusAsync(container)));
                }

                serverViewModels.Add(viewModel);
            }

            return serverViewModels.ToArray();
        }

        public async Task<Server> GetServerByGuidAsync(Guid guid)
        {
           return await DbService.Servers.FindByGuidAsync(guid);
        }

        public async Task CreateServerAsync(Server server)
        {
            foreach (var container in server.Containers)
            {
                if (await DockerService.CreateContainerAsync(container))
                {
                    await DbService.Containers.SaveAsync(container);
                } 
                else
                {
                    await CleanUpFailedServer(server);

                    return;
                }
            }

            await DbService.Servers.SaveAsync(server);
        }

        private async Task CleanUpFailedServer(Server server)
        {
            foreach (var container in server.Containers)
            {
                if (!string.IsNullOrWhiteSpace(container.DockerId))
                {
                    await DockerService.DeleteContainerAsync(container.DockerId);
                }

                if (container.Id != default)
                {
                    await DbService.Containers.DeleteAsync(container);
                }
            }

            if (server.Id != default)
            {
                await DbService.Servers.DeleteAsync(server);
            }            
        }

        public async Task DeleteServerAsync(Server server)
        {
            var containers = await DbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await DockerService.DeleteContainerAsync(container.DockerId, true);
                await DbService.Containers.DeleteAsync(container);
            }

            await DbService.Servers.DeleteAsync(server);
        }

        public async Task RestartServerAsync(Server server)
        {
            await StopServerAsync(server);
            await StartServerAsync(server);
        }

        public async Task StartServerAsync(Server server)
        {
            var containers = await DbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await DockerService.RunContainerAsync(container.DockerId);
            }
        }

        public async Task StopServerAsync(Server server)
        {
            var containers = await DbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await DockerService.StopContainerAsync(container.DockerId);
            }
        }
    }
}
