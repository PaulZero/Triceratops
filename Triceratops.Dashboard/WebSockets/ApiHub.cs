using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.ResponseModels;

namespace Triceratops.Dashboard.WebSockets
{
    public class ApiHub : Hub
    {
        public const string OperationCompleteMethod = "OperationComplete";
        public const string ServerDetailsReceivedMethod = "ServerDetailsReceived";

        private ITriceratopsApiClient _apiClient;

        public ApiHub(ITriceratopsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task StartServerAsync(Guid serverId)
        {
            try
            {
                var response = await _apiClient.StartServerAsync(serverId);

                await Clients.Caller.SendAsync(OperationCompleteMethod, SerialiseResponse(response));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(OperationCompleteMethod, CreateErrorResponse($"Unable to start server: {exception.Message}"));
            }
        }

        public async Task StopServerAsync(Guid serverId)
        {
            try
            {
                var response = await _apiClient.StopServerAsync(serverId);

                await Clients.Caller.SendAsync(OperationCompleteMethod, SerialiseResponse(response));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(OperationCompleteMethod, CreateErrorResponse($"Unable to stop server: {exception.Message}"));
            }
        }

        public async Task RestartServerAsync(Guid serverId)
        {
            try
            {
                var response = await _apiClient.RestartServerAsync(serverId);

                await Clients.Caller.SendAsync(OperationCompleteMethod, SerialiseResponse(response));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(OperationCompleteMethod, CreateErrorResponse($"Unable to restart server: {exception.Message}"));
            }
        }

        public async Task DeleteServerAsync(Guid serverId)
        {
            try
            {
                var response = await _apiClient.DeleteServerAsync(serverId);

                await Clients.Caller.SendAsync(OperationCompleteMethod, SerialiseResponse(response));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(OperationCompleteMethod, CreateErrorResponse($"Unable to delete server: {exception.Message}"));
            }
        }

        public async Task GetServerDetailsAsync(Guid serverId)
        {
            try
            {
                var response = await _apiClient.GetServerByIdAsync(serverId);

                await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, SerialiseResponse(response));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, SerialiseResponse(new ServerDetailsResponse
                {
                    Success = false,
                    Error = $"Unable to get server details: {exception.Message}"
                }));
            }
        }

        private string CreateErrorResponse(string error) => SerialiseResponse(new ServerOperationResponse
        {
            Success = false,
            Message = error
        });

        private string SerialiseResponse(object response)
        {
            return JsonConvert.SerializeObject(response);
        }
    }
}
