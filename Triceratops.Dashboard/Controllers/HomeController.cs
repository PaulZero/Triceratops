using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Triceratops.Dashboard.Models;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;

namespace Triceratops.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITriceratopsApiClient _apiClient;

        private readonly ITriceratopsStorageClient _storageClient;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ITriceratopsApiClient apiClient, ITriceratopsStorageClient storageClient, ILogger<HomeController> logger)
        {
            _apiClient = apiClient;
            _storageClient = storageClient;
            _logger = logger;
        }

        [HttpGet("/", Name = "Home")]
        public async Task<IActionResult> Index()
        {
            var servers = await _apiClient.Servers.GetServerListAsync();

            return View(servers);
        }

        [HttpGet("/servers/{guid}/start", Name = "StartServer")]
        public async Task<IActionResult> StartServer(Guid guid)
        {
            var response = await _apiClient.Servers.StartServerAsync(guid);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/servers/{guid}/stop", Name = "StopServer")]
        public async Task<IActionResult> StopServer(Guid guid)
        {
            await _apiClient.Servers.StopServerAsync(guid);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/servers/{guid}/restart", Name = "RestartServer")]
        public async Task<IActionResult> RestartServer(Guid guid)
        {
            await _apiClient.Servers.RestartServerAsync(guid);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
