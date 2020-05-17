using Microsoft.AspNetCore.Mvc;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { Message = "This is the API, you probably want to go to the dashboard."});
        }
    }
}
