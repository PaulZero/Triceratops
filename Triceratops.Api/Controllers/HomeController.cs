using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triceratops.Api.Services.ServerService;
using Newtonsoft.Json;
using System.Text;
using Triceratops.Api.Models.Servers.Minecraft;
using MongoDB.Driver;
using System.Linq;
using Triceratops.Api.Services.DockerService;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        private IServerService ServerService { get; }

        private IDockerService DockerService { get; }

        public HomeController(IServerService serverService, IDockerService dockerService)
        {
            ServerService = serverService;
            DockerService = dockerService;
        }

        public async Task<IActionResult> Index()
        {
            var servers = await ServerService.GetServersAsync();

            var poop = JsonConvert.SerializeObject(servers, Formatting.Indented);

            return Content(poop, "application/json", Encoding.UTF8);
        }

        [Route("/create-minecraft-server")]
        public async Task<IActionResult> CreateMinecraftServer()
        {
            var config = new MinecraftConfiguration
            {
                ServerName = "Steve",
                MaxPlayers = 16
            };

            var server = await MinecraftServer.CreateAsync(config, ServerService);

            return Json(new
            {
                ok = true
            });
        }

        [Route("/start-server")]
        public async Task<IActionResult> StartServer()
        {
            var servers = await ServerService.GetServersAsync();

            if (servers.Any())
            {
                //await ServerService.StartServerAsync(servers.First());

                return Json(new { ok = true, message = "A server has been started..." });
            }

            return Json(new { ok = false, message = "No servers exist..." });
        }
    }
}
