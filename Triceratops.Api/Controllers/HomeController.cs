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
using Triceratops.Api.Models.StackConfiguration;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IDockerService _dockerService;

        private static bool hasRun;

        public HomeController(ILogger<HomeController> logger, IDockerService dockerService)
        {
            _logger = logger;
            _dockerService = dockerService;
        }

        public async Task<IActionResult> Index()
        {
            if (!hasRun)
            {
                var mc = new MinecraftStackConfiguration(_dockerService, "jons-house-of-weasels");
                await mc.Build();
                await mc.Start();
                await mc.Stop();
                await mc.Destroy();
                hasRun = true;
            }

            return Json(new { ok = true });
        }
    }
}
