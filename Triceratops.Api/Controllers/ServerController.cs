using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Api.Request;
using Triceratops.Libraries.Models.Api.Response;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;

namespace Triceratops.Api.Controllers
{
    public class ServerController : AbstractApiController
    {
        protected IServerService Servers { get; }

        protected IDockerService Docker { get; }

        public ServerController(IServerService serverService, IDockerService dockerService)
        {
            Docker = dockerService;
            Servers = serverService;
        }

        [HttpGet("/servers/list")]
        public async Task<ServerResponse[]> ListServers()
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

        [HttpGet("/servers/by-guid/{guid}")]
        public async Task<ServerResponse> GetServerByGuid(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);

                return await CreateResponseFromServerAsync(server);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to fetch server: {exception.Message}");
            }
        }

        [HttpGet("/servers/by-slug/{slug}")]
        public async Task<ServerResponse> GetServerByName(string slug)
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

        [HttpPost("/servers/{guid}/start")]
        public async Task<IActionResult> StartServer(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);

                await Servers.StartServerAsync(server);

                return Success("Server started successfully");
            }
            catch (Exception exception)
            {
                return Error($"Failed to start server: {exception.Message}");
            }
        }

        [HttpPost("/servers/{guid}/stop")]
        public async Task<IActionResult> StopServer(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);

                await Servers.StopServerAsync(server);

                return Success("Server stopped successfully");
            }
            catch (Exception exception)
            {
                return Error($"Failed to stop server: {exception.Message}");
            }
        }

        [HttpPost("/servers/{guid}/restart")]
        public async Task<IActionResult> RestartServer(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);

                await Servers.RestartServerAsync(server);

                return Success("Server restarted successfully");
            }
            catch (Exception exception)
            {
                return Error($"Failed to restart server: {exception.Message}");
            }
        }

        [HttpPost("/servers/create")]
        public async Task<ServerResponse> CreateServer([FromBody]CreateServerRequest request)
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

        [HttpPost("/servers/{guid}/delete")]
        public async Task<IActionResult> DeleteServer(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);

                await Servers.DeleteServerAsync(server);

                return Success("Server deleted successfully");
            }
            catch (Exception exception)
            {
                return Error($"Failed to delete server: {exception.Message}");
            }
        }

        private async Task<ServerResponse> CreateResponseFromServerAsync(Server server)
        {
            var response = new ServerResponse(server);

            foreach (var container in server.Containers)
            {
                var details = await Docker.GetContainerStatusAsync(container);

                response.Containers.Add(new ContainerResponse(container, details.State, details.Created));
            }

            return response;
        }
    }
}
