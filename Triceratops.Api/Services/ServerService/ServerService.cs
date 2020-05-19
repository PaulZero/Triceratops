using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Api.Models.Servers.Terraria;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Api.Services.DockerService;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Services.ServerService
{
    public class ServerService : IServerService
    {
        private readonly IDbService _dbService;

        private readonly IDockerService _dockerService;

        private readonly ITriceratopsStorageClient _storageClient;

        public ServerService(IDbService dbService, IDockerService dockerService, ITriceratopsStorageClient storageClient)
        {
            _dbService = dbService;
            _dockerService = dockerService;
            _storageClient = storageClient;

            UpdateVolumeServerAsync().Wait();
        }

        public ServerBuilder GetServerBuilder(AbstractServerConfiguration configuration)
        {
            return new ServerBuilder(configuration, new ServerValidator(_dbService));
        }

        public async Task<Server[]> GetServerListAsync()
        {
            return await _dbService.Servers.FindAllAsync();
        }

        public async Task<Dictionary<Guid, string[]>> GetServerLogsAsync(Guid serverId, uint rows)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(serverId);

            var groupedLogs = await Task.WhenAll(containers.Select(async c =>
            {
                var logs = await _dockerService.GetContainerLogAsync(c.DockerId, rows);

                return (c.Id, logs);
            }));

            return groupedLogs.ToDictionary(g => g.Id, g => g.logs);
        }

        public async Task<Server> GetServerByIdAsync(Guid serverId)
        {
            return await _dbService.Servers.FindByIdAsync(serverId);
        }

        public async Task<Server> GetServerBySlugAsync(string slug)
        {
            return await _dbService.Servers.FindBySlugAsync(slug);
        }

        public async Task CreateServerAsync(Server server)
        {
            foreach (var container in server.Containers)
            {
                if (await _dockerService.CreateContainerAsync(container))
                {
                    await _dbService.Containers.SaveAsync(container);
                }
                else
                {
                    await CleanUpFailedServer(server);

                    return;
                }
            }

            await _dbService.Servers.SaveAsync(server);

            if (server.HasVolumes)
            {
                // Refresh!
            }
        }

        public async Task DeleteServerAsync(Server server)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await _dockerService.DeleteContainerAsync(container.DockerId, true);
                await _dbService.Containers.DeleteAsync(container);
            }

            await _dbService.Servers.DeleteAsync(server);

            await UpdateVolumeServerAsync();
        }

        public async Task RestartServerAsync(Server server)
        {
            await StopServerAsync(server);
            await StartServerAsync(server);
        }

        public async Task StartServerAsync(Server server)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await _dockerService.RunContainerAsync(container.DockerId);
            }
        }

        public async Task StopServerAsync(Server server)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await _dockerService.StopContainerAsync(container.DockerId);
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
                    await _dockerService.DeleteContainerAsync(container.DockerId);
                }

                if (container.Id != default)
                {
                    await _dbService.Containers.DeleteAsync(container);
                }
            }

            if (server.Id != default)
            {
                await _dbService.Servers.DeleteAsync(server);
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
            await _dockerService.UpdateVolumeServerAsync(await _dbService.Servers.FindAllAsync());

            await WaitForVolumeServerAsync(10);
        }

        private async Task WaitForVolumeServerAsync(int maxRetries)
        {
            var retries = 0;

            do
            {
                try
                {
                    await _storageClient.GetServerNamesAsync();
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
                finally
                {
                    retries++;
                }
            }
            while (retries < maxRetries);

        }
    }
}
