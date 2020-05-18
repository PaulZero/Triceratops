using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Core;

namespace Triceratops.Libraries.Http.Api
{
    public class TriceratopsApiClient : AbstractHttpClient, ITriceratopsApiClient
    {
        public TriceratopsApiClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
            Client.SetBaseUrl(TriceratopsConstants.InternalApiUrl);
        }

        public Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId)
            => Client.GetAsync<ServerDetailsResponse>($"/servers/by-guid/{serverId}");

        public Task<ServerDetailsResponse> GetServerBySlugAsync(string slug)
            => Client.GetAsync<ServerDetailsResponse>($"/servers/by-slug/{slug}");

        public Task<ServerDetailsResponse[]> GetServerListAsync()
            => Client.GetAsync<ServerDetailsResponse[]>("/servers/list");

        public Task<ServerLogResponse> GetServerLogsAsync(Guid serverId)
            => Client.GetAsync<ServerLogResponse>($"/servers/{serverId}/logs");

        public Task<ServerOperationResponse> StartServerAsync(Guid serverId)
            => Client.PostAsync<ServerOperationResponse>($"/servers/{serverId}/start", null);

        public Task<ServerOperationResponse> StopServerAsync(Guid serverId)
            => Client.PostAsync<ServerOperationResponse>($"/servers/{serverId}/stop", null);

        public Task<ServerOperationResponse> RestartServerAsync(Guid serverId)
            => Client.PostAsync<ServerOperationResponse>($"/servers/{serverId}/restart", null);

        public Task<ServerOperationResponse> DeleteServerAsync(Guid serverId)
            => Client.PostAsync<ServerOperationResponse>($"/servers/{serverId}/delete", null);

        public Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest request)
            => Client.PostAsync<ServerDetailsResponse>($"/servers/create", request);
    }
}
