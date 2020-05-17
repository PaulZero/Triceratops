using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<Server[]> GetServerListAsync();

        Task<Server> GetServerByIdAsync(Guid guid);

        Task<Server> CreateServerFromConfigurationAsync(AbstractServerConfiguration configuration);

        Task CreateServerAsync(Server server);

        Task StartServerAsync(Server server);

        Task StopServerAsync(Server server);

        Task RestartServerAsync(Server server);

        Task DeleteServerAsync(Server server);

        Task<Container[]> GetContainersForServer(Server server);
    }
}
