using CoreRCON;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.DockerService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Interfaces;
using Triceratops.Libraries.Http.Api.Interfaces.Server;
using Triceratops.Libraries.Http.Api.Models;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Api.Controllers
{
    public class ServerController : AbstractApiController, ITriceratopsServerEndpoints
    {
        private readonly IServerService _servers;

        private readonly ITriceratopsDockerClient _dockerService;

        private readonly ILogger _logger;

        public ServerController(IServerService serverService, ITriceratopsDockerClient dockerService, ILogger<ServerController> logger)
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
                var serverDetails = await Task.WhenAll(servers.Select(async s =>
                {
                    var containerDetails = await CreateContainerDetailsForServer(s);
                    var isRunning = containerDetails.All(c => c.State == ServerContainerState.Running);

                    return new ServerBasicDetails(s, isRunning);
                }));

                return new ServerListResponse
                {
                    Success = true,
                    Servers = serverDetails
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to get list of servers: {exception.Message}");

                return new ServerListResponse
                {
                    Success = false,
                    Error = $"Failed to get list of servers: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.GetServerById)]
        public async Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId)
        {
            try
            {
                var server = await _servers.GetServerByIdAsync(serverId);

                return new ServerDetailsResponse
                {
                    Success = true,
                    Server = new ServerExtendedDetails(server, await CreateContainerDetailsForServer(server))
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to fetch server: {exception.Message}");

                return new ServerDetailsResponse
                {
                    Success = false,
                    Error = $"Failed to fetch server: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.GetServerBySlug)]
        public async Task<ServerDetailsResponse> GetServerBySlugAsync(string slug)
        {
            try
            {
                var server = await _servers.GetServerBySlugAsync(slug);

                return new ServerDetailsResponse
                {
                    Success = true,
                    Server = new ServerExtendedDetails(server, await CreateContainerDetailsForServer(server))
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to fetch server: {exception.Message}");

                return new ServerDetailsResponse
                {
                    Success = false,
                    Error = $"Failed to fetch server: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.StartServer)]
        public async Task<ServerOperationResponse> StartServerAsync(Guid serverId)
        {
            try
            {
                await _servers.StartServerAsync(serverId);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = true
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to start server: {exception.Message}");

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = false,
                    Error = $"Failed to start server: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.StopServer)]
        public async Task<ServerOperationResponse> StopServerAsync(Guid serverId)
        {
            try
            {
                await _servers.StopServerAsync(serverId);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = true
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to stop server: {exception.Message}");

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = false,
                    Error = $"Failed to stop server: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.RestartServer)]
        public async Task<ServerOperationResponse> RestartServerAsync(Guid serverId)
        {
            try
            {
                await _servers.RestartServerAsync(serverId);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = true
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to restart server: {exception.Message}");

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = false,
                    Error = $"Failed to restart server: {exception.Message}"
                };
            }
        }

        [ServerApiRoute(ServerApiRoutes.DeleteServer)]
        public async Task<ServerOperationResponse> DeleteServerAsync(Guid serverId)
        {
            try
            {
                await _servers.DeleteServerAsync(serverId);

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = true
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to delete server: {exception.Message}");

                return new ServerOperationResponse
                {
                    ServerId = serverId,
                    Success = false,
                    Error = $"Failed to delete server: {exception.Message}"
                };
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

                    return new ServerDetailsResponse
                    {
                        Success = true,
                        Server = new ServerExtendedDetails(server, await CreateContainerDetailsForServer(server))
                    };
                }

                throw new Exception($"Unsupported configuration: {request.ConfigurationTypeName}");
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to create new server: {exception.Message}");

                return new ServerDetailsResponse
                {
                    Success = false,
                    Error = $"Failed to create new server: {exception.Message}"
                };
            }
        }
        
        /// <summary>
        /// Jon's vile machinations, fucked up by Paul every god damn commit.
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        [HttpGet("/servers/{guid}/rcon")]
        [HttpPost("/servers/{guid}/rcon")]
        public async Task<IActionResult> ExecuteRconCommand(Guid serverId)
        {
            // TODO: Put this somewhere sensible
            // TODO: Handle non-Minecraft servers
            // TODO: Figure out how to stream responses and events
            var server = await _servers.GetServerByIdAsync(serverId);
            var config = server.DeserialiseConfiguration() as MinecraftConfiguration;
            var containerPort = config.RconContainerPort;
            // TODO: Is this reliable?

            var container = server
                .Containers
                .Find(c => c.ServerPorts.Any(p => p.ContainerPort == containerPort));

            if (container == null)
            {
                // TODO: Remove this once the Rcon port is properly exposed
                container = server.Containers.First();
            }

            // TODO: Don't hardcode this
            var ipAddresses = await Dns.GetHostAddressesAsync($"TRICERATOPS_{container.Name}");

            var rcon = new RCON(ipAddresses[0], containerPort, "testing");
            await rcon.ConnectAsync();

            string help = await rcon.SendCommandAsync("help");

            return Json($"We got help from the server: {help}");
        }

        private async Task<IEnumerable<ContainerBasicDetails>> CreateContainerDetailsForServer(Server server)
        {
            return await Task.WhenAll(server.Containers.Select(async c =>
            {
                var details = await _dockerService.GetContainerStatusAsync(c);

                if (!details.Success)
                {
                    throw new Exception($"Unable to read details for container {c.Id}");
                }

                return new ContainerBasicDetails(c, details.State, details.Created);
            }));
        }
    }
}
