using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.ApiService.Interfaces;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;

namespace Triceratops.Dashboard.Controllers
{
    public class MinecraftController : Controller
    {
        private readonly IApiService _apiService;

        public MinecraftController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("/minecraft/create", Name = "Minecraft_Create_Form")]
        [HttpPost("/minecraft/create", Name = "Minecraft_Save_Form")]
        public async Task<IActionResult> Create([FromForm]MinecraftConfiguration configuration = null)
        {
            if (Request.HasFormContentType)
            {
                if (configuration?.IsValid() ?? false)
                {
                    await _apiService.Servers.CreateServerAsync(configuration);

                    return RedirectToRoute("Home");
                }
            }

            return View("Servers/Create", configuration ?? new MinecraftConfiguration());
        }
    }
}
