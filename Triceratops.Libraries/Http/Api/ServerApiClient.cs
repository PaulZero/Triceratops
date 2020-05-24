using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.Exceptions;
using Triceratops.Libraries.Http.Api.Interfaces;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Libraries.Http.Api
{
    internal class ServerApiClient : AbstractHttpClient<ServerApiRoutes>, IServerApiClient
    {
        public ServerApiClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
        }

        public Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId) =>
            Client
            .GetAsync<ServerDetailsResponse>(GetRelativeUrl(ServerApiRoutes.GetServerById, new { serverId }))
            .ValidateApiResponse("Failed to get server by ID");

        public Task<ServerDetailsResponse> GetServerBySlugAsync(string slug) =>
            Client
            .GetAsync<ServerDetailsResponse>(GetRelativeUrl(ServerApiRoutes.GetServerBySlug, new { slug }))
            .ValidateApiResponse("Failed to get server by slug");

        public Task<ServerListResponse> GetServerListAsync() =>
            Client
            .GetAsync<ServerListResponse>(GetRelativeUrl(ServerApiRoutes.GetServerList))
            .ValidateApiResponse("Failed to get server list");

        public Task<ServerOperationResponse> StartServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ServerApiRoutes.StartServer, new { serverId }))
            .ValidateApiResponse("Failed to start server");

        public Task<ServerOperationResponse> StopServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ServerApiRoutes.StopServer, new { serverId }))
            .ValidateApiResponse("Failed to stop server");

        public Task<ServerOperationResponse> RestartServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ServerApiRoutes.RestartServer, new { serverId }))
            .ValidateApiResponse("Failed to restart server");

        public Task<ServerOperationResponse> DeleteServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ServerApiRoutes.DeleteServer, new { serverId }))
            .ValidateApiResponse("Failed to delete server");

        public Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest request) =>
            Client
            .PostAsync<ServerDetailsResponse>(GetRelativeUrl(ServerApiRoutes.CreateServer), request)
            .ValidateApiResponse("Failed to create new server");
    }

    internal static class ServerApiClientTaskExtensions
    {
        public static async Task<T> ValidateApiResponse<T>(this Task<T> task, string validationError)
            where T : AbstractEndpointResponse
        {
            var response = await task;

            if (response == null)
            {
                throw new ApiResponseFailureException($"{validationError}: Failed to receive valid response from API");
            }

            if (!response.Success)
            {
                throw new ApiResponseFailureException(validationError);
            }

            return response;
        }
    }
}
