using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.RequestModels;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerCreationController : Controller
    {
        private readonly ITriceratopsApiClient _apiClient;

        public ServerCreationController(ITriceratopsApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [Route("/server/create/{serverType}", Name = "Server_Create")]
        public IActionResult Create(ServerType serverType)
        {
            var factory = new ServerConfigurationFactory();

            return View(factory.CreateFromServerType(serverType));
        }

        [HttpPost("/server/save", Name = "Server_Save")]
        public async Task<IActionResult> Save()
        {
            var formFields = Request.Form.ToDictionary(f => f.Key, f => f.Value.ToString());

            try
            {
                var factory = new ServerConfigurationFactory();
                var configuration = factory.CreateFromDictionary(formFields);

                await _apiClient.Servers.CreateServerAsync(new CreateServerRequest(configuration));
            }
            catch (Exception exception)
            {
                var poop = 56;
            }

            return RedirectToRoute("Home");
        }
    }
}