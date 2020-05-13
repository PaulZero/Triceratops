using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.View;

namespace Triceratops.Dashboard.Services.ApiService.Interfaces
{
    public interface IServerApi
    {
        Task<ServerViewModel[]> GetServerListAsync();

        Task<ServerViewModel> GetServerByIdAsync(Guid guid);

        Task<bool> StartServerAsync(Guid guid);

        Task<bool> StopServerAsync(Guid guid);

        Task<bool> RestartServerAsync(Guid guid);
    }
}
