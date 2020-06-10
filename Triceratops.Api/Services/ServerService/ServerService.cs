using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Api.Models.Servers.Terraria;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.DockerService;
using Triceratops.DockerService.Models;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Services.ServerService
{
    public class ServerService : IServerService
    {
        private readonly IDbService _dbService;

        private readonly ITriceratopsDockerClient _dockerService;

        public ServerService(IDbService dbService, ITriceratopsDockerClient dockerService)
        {
            _dbService = dbService;
            _dockerService = dockerService;
        }

        public ServerBuilder GetServerBuilder(AbstractServerConfiguration configuration)
        {
            return new ServerBuilder(configuration, new ServerValidator(_dbService));
        }

        public async Task<Server[]> GetServerListAsync()
        {
            return await _dbService.Servers.FindAllAsync();
        }

        public async Task<Stream> GetServerLogStreamAsync(Guid serverId)
        {
            var server = await _dbService.Servers.FindByIdAsync(serverId);
            var container = server.Containers.First();

            var response = await _dockerService.GetContainerLogStreamAsync(container.DockerId);

            if (!response.Success)
            {
                throw new Exception("Unable to fetch log stream for server");
            }

            return response.Stream;
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
            using var transaction = _dbService.StartTransaction();

            try
            {
                foreach (var container in server.Containers)
                {
                    var dockerResponse = await _dockerService.CreateContainerAsync(container);

                    if (!dockerResponse.Success)
                    {
                        throw new Exception($"Failed to create container {container.DisplayName}");
                    }

                    await _dbService.Containers.SaveAsync(container);

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                await _dbService.Servers.SaveAsync(server);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
        }

        public async Task<TemporaryStorageContainer> GetStorageContainerAsync(Guid serverId)
        {
            var server = await GetServerByIdAsync(serverId);
            var response = await _dockerService.GetStorageContainerAsync(server);

            if (response.Success)
            {
                return response.Container;
            }

            throw new Exception("No storage container received from Docker service");
        }

        public async Task DeleteServerAsync(Guid serverId)
            => await DeleteServerAsync(await GetServerByIdAsync(serverId));

        public async Task DeleteServerAsync(Server server)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await _dockerService.DeleteContainerAsync(container);
                await _dbService.Containers.DeleteAsync(container);
            }

            await _dbService.Servers.DeleteAsync(server);
        }

        public async Task RestartServerAsync(Guid serverId)
            => await RestartServerAsync(await GetServerByIdAsync(serverId));

        public async Task RestartServerAsync(Server server)
        {
            await StopServerAsync(server);
            await StartServerAsync(server);
        }

        public async Task StartServerAsync(Guid serverId)
            => await StartServerAsync(await GetServerByIdAsync(serverId));

        public async Task StartServerAsync(Server server)
        {
            var containers = await _dbService.Containers.FindByServerIdAsync(server.Id);

            foreach (var container in containers)
            {
                await _dockerService.StartContainerAsync(container.DockerId);
            }
        }

        public async Task StopServerAsync(Guid serverId)
            => await StopServerAsync(await GetServerByIdAsync(serverId));

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

                return wrappedServer.ServerEntity;
            }

            if (configuration is TerrariaConfiguration terrariaConfiguration)
            {
                var wrappedServer = await TerrariaServer.CreateAsync(terrariaConfiguration, this);

                return wrappedServer.ServerEntity;
            }

            throw new Exception($"Unrecognised server configuration: {configuration.GetType().Name}");
        }

        public async Task<Container[]> GetContainersForServer(Server server)
        {
            return await _dbService.Containers.FindByServerIdAsync(server.Id);
        }

        private async Task CleanUpFailedServer(Server server)
        {
            foreach (var container in server.Containers)
            {
                if (!string.IsNullOrWhiteSpace(container.DockerId))
                {
                    await _dockerService.DeleteContainerAsync(container);
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
    }
}
