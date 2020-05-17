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

        Task<ServerResponse> GetServerByIdAsync(Guid guid);

        Task<ServerResponse> GetServerBySlugAsync(string slug);

        Task<ServerResponse> CreateServerAsync(AbstractServerConfiguration configuration);

        Task<bool> StartServerAsync(Guid guid);

        Task<bool> StopServerAsync(Guid guid);

        Task<bool> RestartServerAsync(Guid guid);
    }
}
