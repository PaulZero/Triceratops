using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.ApiService.Interfaces;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Api.Request;
using Triceratops.Libraries.Models.Api.Response;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Dashboard.Services.ApiService
{
    public class ServerApi : AbstractApiClient, IServerApi
    {
        public ServerApi(string baseUrl, ILogger<IServerApi> logger) : base(baseUrl, logger)
        {
        }

        public Task<ServerResponse> GetServerByIdAsync(Guid serverId)
            => GetAsync<ServerResponse>($"/servers/by-guid/{serverId}");

        public Task<ServerResponse> GetServerBySlugAsync(string slug)
            => GetAsync<ServerResponse>($"/servers/by-slug/{slug}");

        public Task<ServerResponse[]> GetServerListAsync()
            => GetAsync<ServerResponse[]>("/servers/list");

        public Task<ServerLogResponse> GetServerLogsAsync(Guid serverId)
            => GetAsync<ServerLogResponse>($"/servers/{serverId}/logs");

        public async Task<bool> StartServerAsync(Guid serverId)
        {
            var response = await PostApiAsync($"/servers/{serverId}/start");

            return response.Success;
        }

        public async Task<bool> StopServerAsync(Guid serverId)
        {
            var response = await PostApiAsync($"/servers/{serverId}/stop");

            return response.Success;
        }

        public async Task<bool> RestartServerAsync(Guid serverId)
        {
            var response = await PostApiAsync($"/servers/{serverId}/restart");

            return response.Success;
        }

        public Task<ServerResponse> CreateServerAsync(AbstractServerConfiguration configuration)
        {
            var request = new CreateServerRequest(configuration);

            return PostModelAsync<ServerResponse>($"/servers/create", request);
        }
    }
}
