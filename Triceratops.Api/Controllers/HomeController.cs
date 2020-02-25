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

        private readonly IDbService _dbService;

        private static bool hasRun;

        public HomeController(ILogger<HomeController> logger, IDockerService dockerService, IDbService dbService)
        {
            _dockerService = dockerService;
            _dbService = dbService;
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

            var stacks = await _dbService.Stacks.FetchAllAsync();

            var result = stacks.Select(s =>
            {
                var containers = s.GetContainersAsync(_dbService.Containers).Result;
                var containerResult = containers.Select(c => new
                {
                    containerId = c.Id,
                    stackId = c.StackId,
                    imageName = c.ImageName
                });

                return new
                {
                    stackId = s.Id,
                    type = s.StackType.Name,
                    containers = containerResult
                };
            });

            return Json(new { ok = true, stacks = result });
        }
    }
}
