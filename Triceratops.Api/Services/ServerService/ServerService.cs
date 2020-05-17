using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Servers;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Api.Models.Servers.Terraria;
using Triceratops.Api.Models.View;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DockerService;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

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

            UpdateVolumeServerAsync().Wait();
        }

        public ServerBuilder GetServerBuilder(AbstractServerConfiguration configuration)
        {
            return new ServerBuilder(configuration, new ServerValidator(DbService));
        }

        public async Task<Server[]> GetServerListAsync()
        {
            return await DbService.Servers.FindAllAsync();
        }

        public async Task<Server> GetServerByIdAsync(Guid guid)
        {
           return await DbService.Servers.FindByIdAsync(guid);
        }

        public async Task<Server> GetServerBySlugAsync(string slug)
        {
            return await DbService.Servers.FindBySlugAsync(slug);
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

            if (server.HasVolumes)
            {
                // Refresh!
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

            await UpdateVolumeServerAsync();
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

        public async Task<Server> CreateServerFromConfigurationAsync(AbstractServerConfiguration configuration)
        {
            await ValidateNewServerConfiguration(configuration);

            if (configuration is MinecraftConfiguration minecraftConfiguration)
            {
                var wrappedServer = await MinecraftServer.CreateAsync(minecraftConfiguration, this);

                await UpdateVolumeServerAsync();

                return wrappedServer.ServerEntity;
            }

            if (configuration is TerrariaConfiguration terrariaConfiguration)
            {
                var wrappedServer = await TerrariaServer.CreateAsync(terrariaConfiguration, this);

                await UpdateVolumeServerAsync();

                return wrappedServer.ServerEntity;
            }

            throw new Exception($"Unrecognised server configuration: {configuration.GetType().Name}");
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

        private async Task ValidateNewServerConfiguration(AbstractServerConfiguration configuration)
        {
            var existingServers = await GetServerListAsync();

            if (existingServers.Any(s => s.HostPorts.Contains(configuration.HostPort)))
            {
                throw new Exception($"Server with port {configuration.HostPort} already exists.");
            }
            
            if (existingServers.Any(s => s.Name == configuration.ServerName))
            {
                throw new Exception($"Server with name {configuration.ServerName} already exists.");
            }
        }

        private async Task UpdateVolumeServerAsync()
        {
            await DockerService.UpdateVolumeServerAsync(await DbService.Servers.FindAllAsync());
        }
    }
}
