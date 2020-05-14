using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triceratops.Dashboard.Services.ApiService.Interfaces;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;

namespace Triceratops.Dashboard.Controllers
{
    public class ServerCreationController : Controller
    {
        private readonly IApiService _apiService;

        public ServerCreationController(IApiService apiService)
        {
            _apiService = apiService;
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
                var config = factory.CreateFromDictionary(formFields);

                await _apiService.Servers.CreateServerAsync(config);
            }
            catch (Exception exception)
            {
                var poop = 56;
            }

            return RedirectToRoute("Home");
        }
    }
}