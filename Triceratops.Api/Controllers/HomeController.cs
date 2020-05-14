using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triceratops.Api.Services.ServerService;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Api.Services.DockerService;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        private IServerService ServerService { get; }

        public HomeController(IServerService serverService)
        {
            ServerService = serverService;
        }

        public IActionResult Index()
        {
            return Json(new { Message = "This is the API, you probably want to go to the dashboard."});
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
    }
}
