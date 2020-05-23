using Microsoft.AspNetCore.Mvc;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json("This is the API");
        }
    }
}
