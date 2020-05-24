using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Models.Responses.WebSockets;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Services;

namespace Triceratops.Api.WebSockets
{
    public class ApiHub : Hub
    {
        public const string OperationCompleteMethod = "OperationComplete";
        public const string ServerDetailsReceivedMethod = "ServerDetailsReceived";

        private readonly IServerService _serverService;

        public ApiHub(IServerService serverService)
        {
            _serverService = serverService;
        }

        public async IAsyncEnumerable<string> ServerLogsAsync(
            [EnumeratorCancellation]
            CancellationToken token
        )
        {
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                }
                catch
                {
                    break;
                }

                yield return "Let us all unite!";

                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }

        }

        public async Task StartServerAsync(Guid serverId)
        {
            try
            {
                await _serverService.StartServerAsync(serverId);

                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.Successful.ToJson()
                );
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.CreateFromError($"Unable to start server: {exception.Message}").ToJson()
                );
            }
        }

        public async Task StopServerAsync(Guid serverId)
        {
            try
            {
                await _serverService.StopServerAsync(serverId);

                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.Successful.ToJson()
                );
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.CreateFromError($"Unable to stop server: {exception.Message}").ToJson()
                );
            }
        }

        public async Task RestartServerAsync(Guid serverId)
        {
            try
            {
                await _serverService.RestartServerAsync(serverId);

                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.Successful.ToJson()
                );
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.CreateFromError($"Unable to restart server: {exception.Message}").ToJson()
                );
            }
        }

        public async Task DeleteServerAsync(Guid serverId)
        {
            try
            {
                await _serverService.DeleteServerAsync(serverId);

                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.Successful.ToJson()
                );
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(
                    OperationCompleteMethod,
                    ReadOnlyServiceResponse.CreateFromError($"Unable to delete server: {exception.Message}").ToJson()
                );
            }
        }

        //public async Task GetServerDetailsAsync(Guid serverId)
        //{
        //    try
        //    {
        //        var response = await _serverService.GetServerByIdAsync(serverId);

        //        await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, SerialiseResponse(response));
        //    }
        //    catch (Exception exception)
        //    {
        //        await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, SerialiseResponse(new ServerDetailsResponse
        //        {
        //            Success = false,
        //            Error = $"Unable to get server details: {exception.Message}"
        //        }));
        //    }
        //}
    }
}
