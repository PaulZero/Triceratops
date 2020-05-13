using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Triceratops.Dashboard.Models;
using Triceratops.Dashboard.Services.ApiService.Interfaces;

namespace Triceratops.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiService _apiService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(IApiService apiService, ILogger<HomeController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var servers = await _apiService.Servers.GetServerListAsync();

            return View(servers);
        }

        [HttpGet("/servers/{guid}/start", Name = "StartServer")]
        public async Task<IActionResult> StartServer(Guid guid)
        {
            await _apiService.Servers.StartServerAsync(guid);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/servers/{guid}/stop", Name = "StopServer")]
        public async Task<IActionResult> StopServer(Guid guid)
        {
            await _apiService.Servers.StopServerAsync(guid);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("/servers/{guid}/restart", Name = "RestartServer")]
        public async Task<IActionResult> RestartServer(Guid guid)
        {
            await _apiService.Servers.RestartServerAsync(guid);

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
