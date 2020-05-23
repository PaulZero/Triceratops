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
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerController : AbstractDashboardController
    {
        private readonly ITriceratopsApiClient _apiService;

        private readonly ILogger _logger;

        public ServerController(ITriceratopsApiClient apiService, ILogger<ServerController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [DashboardRoute(DashboardRoutes.ListServers)]
        public async Task<IActionResult> ListServers()
        {
            var response = await _apiService.Servers.GetServerListAsync();
            var models = response.Servers.Select(s => WrapServerResponseAsync(s));

            return View(await Task.WhenAll(models));
        }

        [DashboardRoute(DashboardRoutes.ViewServerDetails)]
        public async Task<IActionResult> ServerDetails(string slug)
        {
            try
            {
                var server = await _apiService.Servers.GetServerBySlugAsync(slug);
                var model = await WrapServerResponseAsync(server, true, true);

                return View(model);
            }
            catch
            {
                return RedirectToRoute(DashboardRoutes.ListServers);
            }
            
        }

        [DashboardRoute(DashboardRoutes.EditServerFile)]
        public async Task<IActionResult> EditServerFile(string slug, string fileHash)
        {
            var relativeFilePath = HashHelper.CreateString(fileHash);

            var server = await _apiService.Servers.GetServerBySlugAsync(slug);
            var receivedFile = await _apiService.Storage.DownloadFileAsync(server.ServerId, relativeFilePath);

            return View(new ServerFileViewModel
            {
                FileName = receivedFile.FileName,
                FileText = receivedFile.GetFileContents(),
                ServerName = server.Name,
                ServerSlug = server.Slug,
                RelativeFilePath = relativeFilePath
            });
        }

        [DashboardRoute(DashboardRoutes.SaveServerFile)]
        public async Task<IActionResult> SaveServerFile([FromForm]ServerFileViewModel model)
        {
            await _apiService.Storage.UploadFileAsync(model.ServerSlug, model.RelativeFilePath, new MemoryStream(Encoding.UTF8.GetBytes(model.FileText)));

            return RedirectToRoute(DashboardRoutes.ViewServerDetails, new { slug = model.ServerSlug });
        }

        private async Task<ServerViewModel> WrapServerResponseAsync(ServerDetailsResponse response, bool includeStorage = false, bool includeLogs = false)
        {
            var storageResponse = includeStorage ? await _apiService.Storage.GetServerVolumesAsync(response.ServerId) : null;
            var logs = includeLogs ? await _apiService.Servers.GetServerLogsAsync(response.ServerId) : null;

            return new ServerViewModel(
                response,
                storageResponse?.Volumes,
                logs
            );
        }
    }
}
