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
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerController : AbstractDashboardController
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

        [DashboardRoute(DashboardRoutes.ListServers)]
        public async Task<IActionResult> ListServers()
        {
            var response = await _apiService.GetServerListAsync();
            var models = response.Servers.Select(s => WrapServerResponseAsync(s));

            return View(await Task.WhenAll(models));
        }

        [DashboardRoute(DashboardRoutes.ViewServerDetails)]
        public async Task<IActionResult> ServerDetails(string slug)
        {
            try
            {
                var server = await _apiService.GetServerBySlugAsync(slug);
                var model = await WrapServerResponseAsync(server, true, true);

                return View(model);
            }
            catch
            {
                return RedirectToRoute(DashboardRoutes.ListServers);
            }
            
        }

        [DashboardRoute(DashboardRoutes.EditServerFile)]
        public async Task<IActionResult> EditServerFile(string fileHash)
        {
            var relativeFilePath = HashHelper.CreateString(fileHash);
            var slug = relativeFilePath.Split('/').First(s => !string.IsNullOrWhiteSpace(s));

            var server = await _apiService.GetServerBySlugAsync(slug);
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

        [DashboardRoute(DashboardRoutes.SaveServerFile)]
        public async Task<IActionResult> SaveServerFile([FromForm]ServerFileViewModel model)
        {
            await _storageClient.UploadFileAsync(model.RelativeFilePath, new MemoryStream(Encoding.UTF8.GetBytes(model.FileText)));

            return RedirectToRoute(DashboardRoutes.ViewServerDetails, new { slug = model.ServerSlug });
        }

        private async Task<ServerViewModel> WrapServerResponseAsync(ServerDetailsResponse response, bool includeStorage = false, bool includeLogs = false)
        {
            var storage = includeStorage ? await _storageClient.GetServerAsync(response.Slug) : null;
            var logs = includeLogs ? await _apiService.GetServerLogsAsync(response.ServerId) : null;
            var model = new ServerViewModel(response, storage?.Server, logs);

            return model;
        }
    }
}
