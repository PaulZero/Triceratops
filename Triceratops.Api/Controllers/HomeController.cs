using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Triceratops.Api.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Net;
using System.IO;
using Triceratops.Api.Services.DockerService;
using Triceratops.Api.Models.StackConfiguration.Minecraft;
using Microsoft.Extensions.Configuration;
using Triceratops.Api.Services.DbService;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDockerService _dockerService;

        private readonly IDbStackRepo _stackRepo;

        private static bool hasRun;

        public HomeController(ILogger<HomeController> logger, IDockerService dockerService, IDbStackRepo stackRepo)
        {
            _dockerService = dockerService;
            _stackRepo = stackRepo;
        }

        public async Task<IActionResult> Index()
        {
            //if (!hasRun)
            //{
            //    var mc = new MinecraftStackConfiguration(_dockerService, "jons-house-of-weasels");
            //    await mc.BuildAsync();
            //    await mc.StartAsync();
            //    await mc.StopAsync();
            //    await mc.DestroyAsync();
            //    hasRun = true;
            //}

            var stacks = await _stackRepo.FetchAll();

            return Json(new { ok = true, types = stacks.Select(s => s.StackConfigurationType.Name) });
        }
    }
}
