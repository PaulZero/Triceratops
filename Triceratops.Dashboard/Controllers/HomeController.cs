using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Triceratops.Dashboard.Models;
using Triceratops.Dashboard.Services.NotificationService;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;

namespace Triceratops.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITriceratopsApiClient _apiClient;

        private readonly ILogger<HomeController> _logger;

        private readonly INotificationService _notificationService;

        public HomeController(
            ITriceratopsApiClient apiClient,
            ILogger<HomeController> logger,
            INotificationService notificationService
        )
        {
            _apiClient = apiClient;
            _logger = logger;
            _notificationService = notificationService;
        }

        [HttpGet("/", Name = "Home")]
        public async Task<IActionResult> Index()
        {
            var servers = await _apiClient.GetServerListAsync();

            return View(servers);
        }

        [HttpGet("/servers/{slug}/start", Name = "StartServer")]
        public async Task<IActionResult> StartServer(string slug)
        {
            try
            {
                var serverId = await GetGuidFromServerSlug(slug);
                var response = await _apiClient.StartServerAsync(serverId);

                PushMessageForServerOperationResponse(
                    response,
                    "Server started.",
                    $"Server could not be started: {response.Message ?? "No error received"}"
                );
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error starting server {slug}: {exception.Message}");

                PushErrorMessage($"An error occurred when starting the server: {exception.Message}");
            }

            return ReturnToServerDetails(slug);
        }

        [HttpGet("/servers/{slug}/stop", Name = "StopServer")]
        public async Task<IActionResult> StopServer(string slug)
        {
            try
            {
                var serverId = await GetGuidFromServerSlug(slug);
                var response = await _apiClient.StopServerAsync(serverId);

                PushMessageForServerOperationResponse(
                    response,
                    "Server stopped.",
                    $"Server could not be stopped: {response.Message ?? "No error received"}"
                );
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error stopping server {slug}: {exception.Message}");

                PushErrorMessage($"An error occurred when stopping the server: {exception.Message}");
            }
            return ReturnToServerDetails(slug);
        }

        [HttpGet("/servers/{slug}/restart", Name = "RestartServer")]
        public async Task<IActionResult> RestartServer(string slug)
        {
            try
            {
                var serverId = await GetGuidFromServerSlug(slug);
                var response = await _apiClient.RestartServerAsync(serverId);

                PushMessageForServerOperationResponse(
                    response,
                    "Server restarted.",
                    $"Server could not be restarted: {response.Message ?? "No error received"}"
                );
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error restarting server {slug}: {exception.Message}");

                PushErrorMessage($"An error occurred when restarting the server: {exception.Message}");
            }

            return ReturnToServerDetails(slug);
        }

        [HttpGet("/servers/{slug}/delete", Name = "DeleteServer")]
        public async Task<IActionResult> DeleteServer(string slug)
        {
            try
            {
                var serverId = await GetGuidFromServerSlug(slug);
                var response = await _apiClient.DeleteServerAsync(serverId);

                PushMessageForServerOperationResponse(
                    response,
                    "Server deleted.",
                    $"Server could not be deleted: {response.Message ?? "No error received"}"
                );
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error deleting server {slug}: {exception.Message}");

                PushErrorMessage($"An error occurred when deleting the server: {exception.Message}");
            }

            return ReturnToServerDetails(slug);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult ReturnToServerDetails(string slug)
        {
            
            return RedirectToRoute("ServerDetails", new { slug });
        }

        private async Task<Guid> GetGuidFromServerSlug(string slug)
        {
            var server = await _apiClient.GetServerBySlugAsync(slug);

            return server.Id;
        }

        private void PushErrorMessage(string errorMessage)
        {
            _notificationService.PushMessage(errorMessage, MessageLevel.Error);
        }

        private void PushMessageForServerOperationResponse(ServerOperationResponse response, string successMessage, string failMessage)
        {
            _notificationService.PushMessage(
                response.Success ? successMessage : failMessage,
                response.Success ? MessageLevel.Success : MessageLevel.Error
            );
        }
    }
}
