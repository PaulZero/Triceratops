using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;

namespace Triceratops.Api.Controllers
{
    public class ServerController : Controller
    {
        protected IServerService Servers { get; }

        public ServerController(IServerService serverService)
        {
            Servers = serverService;
        }

        [Route("/servers/list")]
        public async Task<IActionResult> ListServers()
        {
            return Json(await Servers.GetServerViewListAsync());
        }

        [Route("/servers/delete/{guid}")]
        public async Task<IActionResult> DeleteServer(Guid guid)
        {
            var server = await Servers.GetServerByGuidAsync(guid);

            if (server == null)
            {
                return Json(new { ok = false, message = "Server does not exist" });
            }

            await Servers.DeleteServerAsync(server);

            return Json(new { ok = false, message = "Server deleted" });
        }
    }
}
