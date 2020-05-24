using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.NotificationService;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerCreationController : AbstractDashboardController
    {
        private readonly ITriceratopsApiClient _apiClient;

        private readonly INotificationService _notificationService;

        private readonly ILogger _logger;

        public ServerCreationController(
            ITriceratopsApiClient apiClient,
            INotificationService notificationService,
            ILogger<ServerCreationController> logger
        )
        {
            _apiClient = apiClient;
            _notificationService = notificationService;
            _logger = logger;
        }

        [DashboardRoute(DashboardRoutes.CreateServer)]
        public IActionResult Create(ServerType serverType)
        {
            var factory = new ServerConfigurationFactory();

            return View(factory.CreateFromServerType(serverType));
        }

        [DashboardRoute(DashboardRoutes.SaveServer)]
        public async Task<IActionResult> Save()
        {
            var formFields = Request.Form.ToDictionary(f => f.Key, f => f.Value.ToString());

            try
            {
                var factory = new ServerConfigurationFactory();
                var configuration = factory.CreateFromDictionary(formFields);

                var createServerResponse = await _apiClient.Servers.CreateServerAsync(new CreateServerRequest(configuration));

                if (!createServerResponse?.Success ?? false)
                {
                    _notificationService.PushMessage("Unable to create new server.", MessageLevel.Error);

                    return RedirectToRoute(DashboardRoutes.Home);
                }   
                
                return RedirectToRoute(DashboardRoutes.ViewServerDetails, new { slug = createServerResponse.Server.Slug });                
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error creating new server: {exception.Message}");

                _notificationService.PushMessage("Error when creating new server.", MessageLevel.Error);
            }

            return RedirectToRoute(DashboardRoutes.Home);
        }
    }
}