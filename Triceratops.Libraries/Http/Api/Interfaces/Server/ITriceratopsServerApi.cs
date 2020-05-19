using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Http.Api.Interfaces.Server
{
    public interface ITriceratopsServerApi
    {
        Task<ServerListResponse> GetServerListAsync();

        Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId);

        Task<ServerLogResponse> GetServerLogsAsync(Guid serverId);

        Task<ServerDetailsResponse> GetServerBySlugAsync(string slug);

        Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest request);

        Task<ServerOperationResponse> StartServerAsync(Guid serverId);

        Task<ServerOperationResponse> StopServerAsync(Guid serverId);

        Task<ServerOperationResponse> RestartServerAsync(Guid serverId);

        Task<ServerOperationResponse> DeleteServerAsync(Guid serverId);
    }
}
