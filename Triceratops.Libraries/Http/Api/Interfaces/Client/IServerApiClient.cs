using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;

namespace Triceratops.Libraries.Http.Api.Interfaces.Client
{
    public interface IServerApiClient
    {
        Task<ServerListResponse> GetServerListAsync();

        Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId);

        Task<ServerDetailsResponse> GetServerBySlugAsync(string slug);

        Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest configuration);

        Task<ServerOperationResponse> StartServerAsync(Guid serverId);

        Task<ServerOperationResponse> StopServerAsync(Guid serverId);

        Task<ServerOperationResponse> RestartServerAsync(Guid serverId);

        Task<ServerOperationResponse> DeleteServerAsync(Guid serverId);
    }
}
