using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Dashboard.Models;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerController : Controller
    {
        private readonly ITriceratopsApiClient _apiService;

        private readonly ILogger _logger;

        private readonly ITriceratopsStorageClient _storageClient;

        public ServerController(ITriceratopsApiClient apiService, ILogger<ServerController> logger, ITriceratopsStorageClient storageClient)
        {
            _apiService = apiService;
            _logger = logger;
            _storageClient = storageClient;
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
            var model = await WrapServerResponseAsync(server, true, true);

            return View(model);
        }

        [HttpGet("/servers/files/edit/{fileHash}", Name = "EditServerFile")]
        public async Task<IActionResult> EditServerFile(string fileHash)
        {
            var relativeFilePath = HashHelper.CreateString(fileHash);
            var slug = relativeFilePath.Split('/').First(s => !string.IsNullOrWhiteSpace(s));

            var server = await _apiService.Servers.GetServerBySlugAsync(slug);
            using var receivedFile = await _storageClient.DownloadFileAsync(relativeFilePath);

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
            await _storageClient.UploadFileAsync(model.RelativeFilePath, new MemoryStream(Encoding.UTF8.GetBytes(model.FileText)));

            return RedirectToRoute("ServerDetails", new { slug = model.ServerSlug });
        }

        private async Task<ServerViewModel> WrapServerResponseAsync(ServerDetailsResponse response, bool includeStorage = false, bool includeLogs = false)
        {
            var storage = includeStorage ? await _storageClient.GetServerAsync(response.Slug) : null;
            var logs = includeLogs ? await _apiService.Servers.GetServerLogsAsync(response.Id) : null;
            var model = new ServerViewModel(response, storage?.Server, logs);

            return model;
        }
    }
}
