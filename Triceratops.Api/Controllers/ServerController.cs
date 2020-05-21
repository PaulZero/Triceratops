using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Interfaces;
using Triceratops.Libraries.Http.Api.Interfaces.Server;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;
using Triceratops.Libraries.RouteMapping;
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;
using static Triceratops.Libraries.Http.Api.ResponseModels.ServerLogResponse;

namespace Triceratops.Api.Controllers
{
    public class ServerController : AbstractApiController, ITriceratopsServerEndpoints
    {
        private readonly IServerService _servers;

        private readonly IDockerService _dockerService;

        private readonly ILogger _logger;

        public ServerController(IServerService serverService, IDockerService dockerService, ILogger<ServerController> logger)
        {
            _dockerService = dockerService;
            _servers = serverService;
            _logger = logger;
        }

        [ServerApiRoute(ServerApiRoutes.GetServerList)]
        public async Task<ServerListResponse> GetServerListAsync()
        {
            try
            {
                var servers = await _servers.GetServerListAsync();
                var responses = await Task.WhenAll(servers.Select(CreateResponseFromServerAsync));

                return new ServerListResponse
                {
                    Success = true,
                    Servers = responses
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to get list of servers: {exception.Message}");

                return CreateErrorResponse<ServerListResponse>("Failed to get list of servers");
            }
        }

        [ServerApiRoute(ServerApiRoutes.GetServerLogs)]
        public async Task<ServerLogResponse> GetServerLogsAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);
                var logDictionary = await _servers.GetServerLogsAsync(serverId, 300);

                return new ServerLogResponse
                {
                    ServerId = server.Id,
                    ServerName = server.Name,
                    ContainerLogItems = server.Containers.Select(c =>
                    {
                        return new ContainerLogItem
                        {
                            ContainerId = c.Id,
                            ContainerName = c.Name,
                            LogRows = logDictionary[c.Id]
                        };
                    }).ToArray()
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to fetch server logs: {exception.Message}");

                return CreateErrorResponse<ServerLogResponse>("Failed to fetch server logs");
            }
        }

        [ServerApiRoute(ServerApiRoutes.GetServerById)]
        public async Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                return await CreateResponseFromServerAsync(server);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to fetch server: {exception.Message}");

                return CreateErrorResponse<ServerDetailsResponse>("Failed to fetch server");
            }
        }

        [ServerApiRoute(ServerApiRoutes.GetServerBySlug)]
        public async Task<ServerDetailsResponse> GetServerBySlugAsync(string slug)
        {
            try
            {
                var server = await _servers.GetServerBySlugAsync(slug);

                return await CreateResponseFromServerAsync(server);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to fetch server: {exception.Message}");

                return CreateErrorResponse<ServerDetailsResponse>("Failed to fetch server");
            }
        }

        [ServerApiRoute(ServerApiRoutes.StartServer)]
        public async Task<ServerOperationResponse> StartServerAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                await _servers.StartServerAsync(server);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(server),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(serverId),
                    Success = false,
                    Message = exception.Message
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.StopServer)]
        public async Task<ServerOperationResponse> StopServerAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                await _servers.StopServerAsync(server);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(server),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(serverId),
                    Success = false,
                    Message = exception.Message
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.RestartServer)]
        public async Task<ServerOperationResponse> RestartServerAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                await _servers.RestartServerAsync(server);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(server),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                return CreateErrorResponse<ServerOperationResponse>(exception.Message);
            }
        }

        [ServerApiRoute(ServerApiRoutes.DeleteServer)]
        public async Task<ServerOperationResponse> DeleteServerAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                await _servers.DeleteServerAsync(server);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    IsRunning = await IsServerRunningAsync(server),
                    Success = true
                };
            }
            catch (Exception exception)
            {
                return CreateErrorResponse<ServerOperationResponse>(exception.Message);
            }
        }

        [ServerApiRoute(ServerApiRoutes.CreateServer)]
        public async Task<ServerDetailsResponse> CreateServerAsync([FromBody]CreateServerRequest request)
        {
            try
            {
                AbstractServerConfiguration configuration = null;

                if (request.ConfigurationType == typeof(MinecraftConfiguration))
                {
                    configuration = JsonHelper.Deserialise<MinecraftConfiguration>(request.JsonConfiguration);
                }
                else if (request.ConfigurationType == typeof(TerrariaConfiguration))
                {
                    configuration = JsonHelper.Deserialise<TerrariaConfiguration>(request.JsonConfiguration);
                }

                if (configuration != null)
                {
                    var server = await _servers.CreateServerFromConfigurationAsync(configuration);

                    return await CreateResponseFromServerAsync(server);
                }

                throw new Exception($"Unsupported configuration: {request.ConfigurationTypeName}");
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to create new server: {exception.Message}");

                return CreateErrorResponse<ServerDetailsResponse>(exception.Message);
            }
        }

        private async Task<ServerDetailsResponse> CreateResponseFromServerAsync(Server server)
        {
            var response = new ServerDetailsResponse(server);

            foreach (var container in server.Containers)
            {
                var details = await _dockerService.GetContainerStatusAsync(container);

                response.Containers.Add(new ContainerResponse(container, details.State, details.Created));
            }

            return response;
        }

        private async Task<bool> IsServerRunningAsync(Guid serverId)
            => await IsServerRunningAsync(await _servers.GetServerByIdAsync(serverId));

        private async Task<bool> IsServerRunningAsync(Server server)
        {
            try
            {
                var containerStatus = await Task.WhenAll(server.Containers.Select(c => _dockerService.GetContainerStatusAsync(c)));

                return containerStatus.All(s => s.State == ServerContainerState.Running);
            }
            catch
            {
                return false;
            }
        }

        private T CreateErrorResponse<T>(string error)
            where T : IApiResponse, new()
        {
            return new T
            {
                Success = false,
                Error = error
            };
        }

        private T CreateErrorResponse<T>(string error, Guid serverId)
            where T : IServerApiResponse, new()
        {
            return new T
            {
                Success = false,
                Error = error,
                ServerId = serverId
            };
        }
    }
}
