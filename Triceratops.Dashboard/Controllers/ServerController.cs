using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Dashboard.Models;
using Triceratops.Dashboard.Services.ApiService.Interfaces;
using Triceratops.Dashboard.Services.VolumeService.Interfaces;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models.Api.Response;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerController : Controller
    {
        private readonly IApiService _apiService;

        private readonly ILogger _logger;

        private readonly IVolumeService _volumeService;

        public ServerController(IApiService apiService, ILogger<ServerController> logger, IVolumeService volumeService)
        {
            _apiService = apiService;
            _logger = logger;
            _volumeService = volumeService;
        }

        [HttpGet("/servers", Name = "ListServers")]
        public async Task<IActionResult> ListServers()
        {
            var servers = await _apiService.Servers.GetServerListAsync();
            var models = servers.Select(s => WrapServerResponseAsync(s));

            return View(await Task.WhenAll(models));
        }

        [HttpGet("/servers/{slug}", Name = "ServerDetails")]
        public async Task<IActionResult> ServerDetails(string slug)
        {
            var server = await _apiService.Servers.GetServerBySlugAsync(slug);
            var model = await WrapServerResponseAsync(server, true);

            return View(model);
        }

        [HttpGet("/servers/{slug}/files/edit/{fileHash}", Name = "EditServerFile")]
        public async Task<IActionResult> EditServerFile(string slug, string fileHash)
        {
            var relativeFilePath = HashHelper.CreateString(fileHash);

            var server = await _apiService.Servers.GetServerBySlugAsync(slug);
            using var receivedFile = await _volumeService.DownloadFileAsync(relativeFilePath);

            return View(new ServerFileViewModel
            {
                FileName = receivedFile.Name,
                FileText = receivedFile.GetStreamAsString(),
                ServerName = server.Name,
                ServerSlug = server.Slug,
                RelativeFilePath = relativeFilePath
            });
        }

        [HttpPost("/servers/files/save", Name = "SaveServerFile")]
        public async Task<IActionResult> SaveServerFile([FromForm]ServerFileViewModel model)
        {
            await _volumeService.UploadFileAsync(model.RelativeFilePath, new MemoryStream(Encoding.UTF8.GetBytes(model.FileText)));

            return RedirectToRoute("ServerDetails", new { slug = model.ServerSlug });
        }

        private async Task<ServerViewModel> WrapServerResponseAsync(ServerResponse response, bool includeStorage = false)
        {
            var storage = includeStorage ? await _volumeService.GetServerAsync(response.Slug) : null;
            var model = new ServerViewModel(response, storage);

            return model;
        }
    }
}
