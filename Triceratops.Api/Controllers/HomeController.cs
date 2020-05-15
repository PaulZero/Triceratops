using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triceratops.Api.Services.ServerService;
using Triceratops.Api.Models.Servers.Minecraft;
using Triceratops.Api.Services.DockerService;
using Triceratops.Libraries.Models.ServerConfiguration.Minecraft;
using System.IO;
using System.Text;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { Message = "This is the API, you probably want to go to the dashboard."});
        }

        [Route("/volume")]
        public IActionResult Volume()
        {
            var directory = new DirectoryInfo("/app/gamedata");
            var stringBuilder = new StringBuilder();

            CreateDirectoryMap(directory, stringBuilder);

            return Content(stringBuilder.ToString());
        }

        private void CreateDirectoryMap(DirectoryInfo directoryInfo, StringBuilder stringBuilder, int depth = 1)
        {
            var indent = new string(' ', depth * 2);

            stringBuilder.AppendLine($"{indent}{directoryInfo.Name}");

            foreach (var directory in directoryInfo.GetDirectories())
            {
                CreateDirectoryMap(directory, stringBuilder, depth + 1);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                stringBuilder.AppendLine($"{indent} - {file.Name}");
            }
        }
    }
}
