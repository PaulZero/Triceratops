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
    public class TriceratopsApiClient : AbstractHttpClient<ApiRoutes>, ITriceratopsApiClient
    {
        public TriceratopsApiClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
            Client.SetBaseUrl(Constants.InternalApiUrl);
        }

        public Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId) =>
            Client
            .GetAsync<ServerDetailsResponse>(GetRelativeUrl(ApiRoutes.GetServerById, new { serverId }))
            .ValidateApiResponse("Failed to get server by ID");

        public Task<ServerDetailsResponse> GetServerBySlugAsync(string slug) =>
            Client
            .GetAsync<ServerDetailsResponse>(GetRelativeUrl(ApiRoutes.GetServerBySlug, new { slug }))
            .ValidateApiResponse("Failed to get server by slug");

        public Task<ServerListResponse> GetServerListAsync() =>
            Client
            .GetAsync<ServerListResponse>(GetRelativeUrl(ApiRoutes.GetServerList))
            .ValidateApiResponse("Failed to get server list");

        public Task<ServerLogResponse> GetServerLogsAsync(Guid serverId) =>
            Client
            .GetAsync<ServerLogResponse>(GetRelativeUrl(ApiRoutes.GetServerLogs, new { serverId }))
            .ValidateApiResponse("Failed to get server logs");

        public Task<ServerOperationResponse> StartServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ApiRoutes.StartServer, new { serverId }))
            .ValidateApiResponse("Failed to start server");

        public Task<ServerOperationResponse> StopServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ApiRoutes.StopServer, new { serverId }))
            .ValidateApiResponse("Failed to stop server");

        public Task<ServerOperationResponse> RestartServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ApiRoutes.RestartServer, new { serverId } ))
            .ValidateApiResponse("Failed to restart server");

        public Task<ServerOperationResponse> DeleteServerAsync(Guid serverId) =>
            Client
            .PostAsync<ServerOperationResponse>(GetRelativeUrl(ApiRoutes.DeleteServer, new { serverId } ))
            .ValidateApiResponse("Failed to delete server");

        public Task<ServerDetailsResponse> CreateServerAsync(CreateServerRequest request) =>
            Client
            .PostAsync<ServerDetailsResponse>(GetRelativeUrl(ApiRoutes.CreateServer), request)
            .ValidateApiResponse("Failed to create new server");
    }

    public static class ApiClientTaskExtensions
    {
        public static async Task<T> ValidateApiResponse<T>(this Task<T> task, string validationError)
            where T : IApiResponse
        {
            var response = await task;

            if (response == null)
            {
                throw new ApiResponseFailureException($"{validationError}: Failed to receive valid response from API");
            }

            if (!response.Success)
            {
                if (response is IServerApiResponse serverResponse)
                {
                    throw new ApiResponseFailureException(validationError, serverResponse);
                }

                throw new ApiResponseFailureException(validationError, response);
            }

            return response;
        }
    }
}
