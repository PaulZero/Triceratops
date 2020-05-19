using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.Interfaces.Server;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;
using static Triceratops.Libraries.Http.Api.ResponseModels.ServerLogResponse;

namespace Triceratops.Api.Controllers
{
    public class ServerController : AbstractApiController, ITriceratopsServerApi
    {
        protected IServerService Servers { get; }

        protected IDockerService Docker { get; }

        public ServerController(IServerService serverService, IDockerService dockerService)
        {
            Docker = dockerService;
            Servers = serverService;
        }

        [HttpGet("/servers/list")]
        public async Task<ServerDetailsResponse[]> GetServerListAsync()
        {
            try
            {
                var servers = await Servers.GetServerListAsync();
                var responses = servers.Select(CreateResponseFromServerAsync);

                return await Task.WhenAll(responses);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to fetch list of servers: {exception.Message}");
            }
        }

        [HttpGet("/servers/{serverId}/logs/{rows?}")]
        public async Task<ServerLogResponse> GetServerLogsAsync(Guid serverId, uint? rows)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);
                var logDictionary = await Servers.GetServerLogsAsync(serverId, rows ?? 300);

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
                throw new Exception($"Failed to fetch server logs: {exception.Message}");
            }
        }

        [HttpGet("/servers/by-guid/{serverId}")]
        public async Task<ServerDetailsResponse> GetServerByIdAsync(Guid serverId)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);

                return await CreateResponseFromServerAsync(server);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to fetch server: {exception.Message}");
            }
        }

        [HttpGet("/servers/by-slug/{slug}")]
        public async Task<ServerDetailsResponse> GetServerBySlugAsync(string slug)
        {
            try
            {
                var server = await Servers.GetServerBySlugAsync(slug);

                return await CreateResponseFromServerAsync(server);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to fetch server: {exception.Message}");
            }
        }

        [HttpPost("/servers/{serverId}/start")]
        public async Task<ServerOperationResponse> StartServerAsync(Guid serverId)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);

                await Servers.StartServerAsync(server);

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

        [HttpPost("/servers/{serverId}/stop")]
        public async Task<ServerOperationResponse> StopServerAsync(Guid serverId)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);

                await Servers.StopServerAsync(server);

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

        [HttpPost("/servers/{serverId}/restart")]
        public async Task<ServerOperationResponse> RestartServerAsync(Guid serverId)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);

                await Servers.RestartServerAsync(server);

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

        [HttpPost("/servers/{serverId}/delete")]
        public async Task<ServerOperationResponse> DeleteServerAsync(Guid serverId)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(serverId);

                await Servers.DeleteServerAsync(server);

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

        [HttpPost("/servers/create")]
        public async Task<ServerDetailsResponse> CreateServerAsync([FromBody]CreateServerRequest request)
        {
            try
            {
                AbstractServerConfiguration configuration = null;

                if (request.ConfigurationType == typeof(MinecraftConfiguration))
                {
                    configuration = JsonConvert.DeserializeObject<MinecraftConfiguration>(request.JsonConfiguration);
                }
                else if (request.ConfigurationType == typeof(TerrariaConfiguration))
                {
                    configuration = JsonConvert.DeserializeObject<TerrariaConfiguration>(request.JsonConfiguration);
                }

                if (configuration != null)
                {
                    var server = await Servers.CreateServerFromConfigurationAsync(configuration);

                    return await CreateResponseFromServerAsync(server);
                }

                throw new Exception($"Unsupported configuration: {request.ConfigurationTypeName}");
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to create new server: {exception.Message}");
            }
        }

        private async Task<ServerDetailsResponse> CreateResponseFromServerAsync(Server server)
        {
            var response = new ServerDetailsResponse(server);

            foreach (var container in server.Containers)
            {
                var details = await Docker.GetContainerStatusAsync(container);

                response.Containers.Add(new ContainerResponse(container, details.State, details.Created));
            }

            return response;
        }

        private async Task<bool> IsServerRunningAsync(Guid serverId)
            => await IsServerRunningAsync(await Servers.GetServerByIdAsync(serverId));

        private async Task<bool> IsServerRunningAsync(Server server)
        {
            try
            {
                var containerStatus = await Task.WhenAll(server.Containers.Select(c => Docker.GetContainerStatusAsync(c)));

                return containerStatus.All(s => s.State == ServerContainerState.Running);
            }
            catch
            {
                return false;
            }
        }
    }
}
