using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.Api.Models.View.Transformers.Interfaces;
using Triceratops.Api.Services.ServerService;
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
    }
}
