using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.DockerService.Models;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Api.Services.ServerService
{
    /// <summary>
    /// The server service (great name) marries the DB and Docker services, ensuring that when
    /// a server is created it is both persisted in the DB and registered with Docker.
    /// </summary>
    public interface IServerService
    {
        ServerBuilder GetServerBuilder(AbstractServerConfiguration configuration);

        Task<Server[]> GetServerListAsync();

        Task<Dictionary<Guid, string[]>> GetServerLogsAsync(Guid serverId, uint rows);

        Task<Server> GetServerByIdAsync(Guid serverId);

        Task<Server> GetServerBySlugAsync(string slug);

        Task<Server> CreateServerFromConfigurationAsync(AbstractServerConfiguration configuration);

        Task<TemporaryStorageContainer> GetStorageContainerAsync(Guid serverId);

        Task CreateServerAsync(Server server);

        Task StartServerAsync(Guid serverId);

        Task StartServerAsync(Server server);

        Task StopServerAsync(Guid serverId);

        Task StopServerAsync(Server server);

        Task RestartServerAsync(Guid serverId);

        Task RestartServerAsync(Server server);

        Task DeleteServerAsync(Guid serverId);

        Task DeleteServerAsync(Server server);

        Task<Container[]> GetContainersForServer(Server server);
    }
}
