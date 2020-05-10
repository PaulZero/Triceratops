using System;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Models.View;

namespace Triceratops.Api.Services.ServerService
{
    /// <summary>
    /// The server service (great name) marries the DB and Docker services, ensuring that when
    /// a server is created it is both persisted in the DB and registered with Docker.
    /// </summary>
    public interface IServerService
    {
        Task<ServerViewModel[]> GetServerViewListAsync();

        Task<Server> GetServerByGuidAsync(Guid guid);

        Task CreateServerAsync(Server server);

        Task StartServerAsync(Server server);

        Task StopServerAsync(Server server);

        Task RestartServerAsync(Server server);

        Task DeleteServerAsync(Server server);
    }
}
