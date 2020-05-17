﻿using CoreRCON;
using CoreRCON.Parsers.Standard;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Models.View.Transformers.Interfaces;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Models.Api.Request;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using Triceratops.Libraries.Models.ServerConfiguration.Terraria;
using Triceratops.Libraries.Models.View;

namespace Triceratops.Api.Controllers
{
    public class ServerController : AbstractApiController
    {
        protected IServerService Servers { get; }

        protected IViewModelTransformer ViewModelTransformer { get; }

        public ServerController(IServerService serverService, IViewModelTransformer viewModelTransformer)
        {
            Servers = serverService;
            ViewModelTransformer = viewModelTransformer;
        }

        [HttpGet("/servers/list")]
        public async Task<IActionResult> ListServers()
        {
            try
            {
                var servers = await Servers.GetServerListAsync();
                var viewModels = await ViewModelTransformer.WrapServersAsync(servers);

                return ViewModel(viewModels);

            }
            catch (Exception exception)
            {
                return Error($"Failed to fetch list of servers: {exception.Message}");
            }
        }

        [HttpGet("/servers/{guid}")]
        public async Task<IActionResult> GetServer(Guid guid)
        {
            try
            {
                var server = await Servers.GetServerByIdAsync(guid);
                var viewModel = await ViewModelTransformer.WrapServerAsync(server);

                return ViewModel(viewModel);
            }
            catch (Exception exception)
            {
                return Error($"Failed to fetch server: {exception.Message}");
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
        public async Task<IActionResult> CreateServer([FromBody]CreateServerRequest request)
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
                    var viewModel = await ViewModelTransformer.WrapServerAsync(server);

                    return ViewModel(viewModel);
                }

                return Error($"Unsupported configuration: {request.ConfigurationTypeName}");
            }
            catch (Exception exception)
            {
                return Error($"Failed to create new server: {exception.Message}");
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

        [HttpGet("/servers/{guid}/rcon")]
        [HttpPost("/servers/{guid}/rcon")]
        public async Task<IActionResult> ServerRconCommand(Guid guid)
        {
            // TODO: Put this somewhere sensible
            // TODO: Handle non-Minecraft servers
            // TODO: Figure out how to stream responses and events
            var server = await Servers.GetServerByIdAsync(guid);
            // TODO: Make it so that server.Containers is actually poopulated instead of doing this
            var containers = await Servers.GetContainersForServer(server);
            var config = server.DeserialiseConfiguration() as MinecraftConfiguration;
            var containerPort = config.RconContainerPort;
            // TODO: Is this reliable?
            var container = containers.ToList().Find(c => c.ServerPorts.Any(p => p.ContainerPort == containerPort));

            if (container == null)
            {
                // TODO: Remove this once the Rcon port is properly exposed
                container = containers.First();
            }

            // TODO: Don't hardcode this
            var ipAddresses = await Dns.GetHostAddressesAsync($"TRICERATOPS_{container.Name}");

            var rcon = new RCON(ipAddresses[0], containerPort, "testing");
            await rcon.ConnectAsync();

            string help = await rcon.SendCommandAsync("help");

            return Success($"We got help from the server: {help}");
        }
    }
}
