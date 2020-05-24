using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver.Core.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.DockerService;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Models;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Services;

namespace Triceratops.Api.WebSockets
{
    public class ApiHub : Hub
    {
        public const string OperationCompleteMethod = "OperationComplete";
        public const string ServerDetailsReceivedMethod = "ServerDetailsReceived";

        private readonly IServerService _serverService;
        private readonly ITriceratopsDockerClient _dockerClient;

        public ApiHub(IServerService serverService, ITriceratopsDockerClient dockerClient)
        {
            _serverService = serverService;
            _dockerClient = dockerClient;
        }

        private async Task<string> ReadLine(StreamReader reader)
        {
            var line = await reader.ReadLineAsync();

            if (line.Length <= 8)
            {
                return null;
            }

            return line.Substring(8);
        }

        public async IAsyncEnumerable<string> ServerLogsAsync(
            Guid serverId,
            [EnumeratorCancellation]
            CancellationToken token
        )
        {
            var stream = await _serverService.GetServerLogStreamAsync(serverId);
            var reader = new StreamReader(stream);

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

                if (reader.EndOfStream)
                {
                    yield return "#ENDOFSTREAM";

                    break;
                }

                var line = await ReadLine(reader);

                if (string.IsNullOrWhiteSpace(line))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));

                    continue;
                }

                yield return line;
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

        public async Task GetServerDetailsAsync(Guid serverId)
        {
            try
            {
                var server = await _serverService.GetServerByIdAsync(serverId);
                var containers = server.Containers.Select(async c =>
                {
                    var details = await _dockerClient.GetContainerStatusAsync(c);
                    return new ContainerBasicDetails(c, details.State, details.Created);
                });

                var hubResponse = new ServerDetailsResponse
                {
                    Success = true,
                    Server = new ServerExtendedDetails(server, await Task.WhenAll(containers))
                };

                await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, JsonHelper.Serialise(hubResponse));
            }
            catch (Exception exception)
            {
                await Clients.Caller.SendAsync(ServerDetailsReceivedMethod, JsonHelper.Serialise(new ServerDetailsResponse
                {
                    Success = false,
                    Error = $"Unable to get server details: {exception.Message}"
                }));
            }
        }
    }
}
