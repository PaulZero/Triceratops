using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Api.Response;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Dashboard.Services.ApiService.Interfaces
{
    public interface IServerApi
    {
        Task<ServerResponse[]> GetServerListAsync();

        Task<ServerResponse> GetServerByIdAsync(Guid serverId);

        Task<ServerLogResponse> GetServerLogsAsync(Guid serverId);

        Task<ServerResponse> GetServerBySlugAsync(string slug);

        Task<ServerResponse> CreateServerAsync(AbstractServerConfiguration configuration);

        Task<bool> StartServerAsync(Guid serverId);

        Task<bool> StopServerAsync(Guid serverId);

        Task<bool> RestartServerAsync(Guid serverId);
    }
}
