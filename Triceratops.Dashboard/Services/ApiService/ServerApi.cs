using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.ApiService.Interfaces;
using Triceratops.Libraries.Models.Api.Request;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.View;

namespace Triceratops.Dashboard.Services.ApiService
{
    public class ServerApi : AbstractApiClient, IServerApi
    {
        public ServerApi(string baseUrl) : base(baseUrl)
        {
        }

        public Task<ServerViewModel> GetServerByIdAsync(Guid guid)
        {
            return GetAsync<ServerViewModel>($"/servers/{guid}");
        }

        public async Task<ServerViewModel[]> GetServerListAsync()
        {
            var items = await GetAsync<List<ServerViewModel>>("/servers/list");

            return items.ToArray();
        }

        public async Task<bool> StartServerAsync(Guid guid)
        {
            var response = await PostAsync($"/servers/{guid}/start");

            return response.Success;
        }

        public async Task<bool> StopServerAsync(Guid guid)
        {
            var response = await PostAsync($"/servers/{guid}/stop");

            return response.Success;
        }

        public async Task<bool> RestartServerAsync(Guid guid)
        {
            var response = await PostAsync($"/servers/{guid}/restart");

            return response.Success;
        }

        public Task<ServerViewModel> CreateServerAsync(AbstractServerConfiguration configuration)
        {
            var request = new CreateServerRequest(configuration);

            return PostAsync<ServerViewModel>($"/servers/create", request);
        }
    }
}
