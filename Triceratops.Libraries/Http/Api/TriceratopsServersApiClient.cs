using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Http.Api
{
    public class TriceratopsServersApiClient : AbstractHttpClient, ITriceratopsServersApiClient
    {
        public TriceratopsServersApiClient(string baseUrl, ILogger logger) : base(baseUrl, logger)
        {
        }

        public Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId)
            => GetAsync<ServerDetailsResponse>($"/servers/by-guid/{serverId}");

        public Task<ServerDetailsResponse> GetServerBySlugAsync(string slug)
            => GetAsync<ServerDetailsResponse>($"/servers/by-slug/{slug}");

        public Task<ServerDetailsResponse[]> GetServerListAsync()
            => GetAsync<ServerDetailsResponse[]>("/servers/list");

        public Task<ServerLogResponse> GetServerLogsAsync(Guid serverId)
            => GetAsync<ServerLogResponse>($"/servers/{serverId}/logs");

        public Task<ServerOperationResponse> StartServerAsync(Guid serverId)
            => PostAsync<ServerOperationResponse>($"/servers/{serverId}/start");

        public Task<ServerOperationResponse> StopServerAsync(Guid serverId)
            => PostAsync<ServerOperationResponse>($"/servers/{serverId}/stop");

        public Task<ServerOperationResponse> RestartServerAsync(Guid serverId)
            => PostAsync<ServerOperationResponse>($"/servers/{serverId}/restart");

        public Task<ServerOperationResponse> DeleteServerAsync(Guid serverId)
            => PostAsync<ServerOperationResponse>($"/servers/{serverId}/delete");

        public Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest request)
            => PostAsync<ServerDetailsResponse>($"/servers/create", request);
    }
}
