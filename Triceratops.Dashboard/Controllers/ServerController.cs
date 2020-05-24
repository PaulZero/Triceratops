using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Dashboard.Models;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
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
            var models = response.Servers.Select(s => new ServerViewModel(s));

            return View(models);
        }

        [DashboardRoute(DashboardRoutes.ViewServerDetails)]
        public async Task<IActionResult> ServerDetails(string slug)
        {
            try
            {
                var serverResponse = await _apiService.Servers.GetServerBySlugAsync(slug);
                var volumeResponse = await _apiService.Storage.GetServerVolumesAsync(slug);

                return View(new ServerViewModel(serverResponse.Server, volumeResponse.Volumes));
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

            var serverResponse = await _apiService.Servers.GetServerBySlugAsync(slug);
            var receivedFile = await _apiService.Storage.DownloadFileAsync(serverResponse.Server.ServerId, relativeFilePath);

            return View(new ServerFileViewModel
            {
                FileName = receivedFile.FileName,
                FileText = receivedFile.GetFileContents(),
                ServerName = serverResponse.Server.Name,
                ServerSlug = serverResponse.Server.Slug,
                RelativeFilePath = relativeFilePath
            });
        }

        [DashboardRoute(DashboardRoutes.SaveServerFile)]
        public async Task<IActionResult> SaveServerFile([FromForm]ServerFileViewModel model)
        {
            await _apiService.Storage.UploadFileAsync(model.ServerSlug, model.RelativeFilePath, new MemoryStream(Encoding.UTF8.GetBytes(model.FileText)));

            return RedirectToRoute(DashboardRoutes.ViewServerDetails, new { slug = model.ServerSlug });
        }
    }
}
