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

        public Task<ServerResponse> GetServerByIdAsync(Guid guid)
            => GetAsync<ServerResponse>($"/servers/by-guid/{guid}");

        public Task<ServerResponse> GetServerBySlugAsync(string slug)
            => GetAsync<ServerResponse>($"/servers/by-slug/{slug}");

        public Task<ServerResponse[]> GetServerListAsync()
            => GetAsync<ServerResponse[]>("/servers/list");

        public async Task<bool> StartServerAsync(Guid guid)
        {
            var response = await PostApiAsync($"/servers/{guid}/start");

            return response.Success;
        }

        public async Task<bool> StopServerAsync(Guid guid)
        {
            var response = await PostApiAsync($"/servers/{guid}/stop");

            return response.Success;
        }

        public async Task<bool> RestartServerAsync(Guid guid)
        {
            var response = await PostApiAsync($"/servers/{guid}/restart");

            return response.Success;
        }

        public Task<ServerResponse> CreateServerAsync(AbstractServerConfiguration configuration)
        {
            var request = new CreateServerRequest(configuration);

            return PostModelAsync<ServerResponse>($"/servers/create", request);
        }
    }
}
