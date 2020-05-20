using Microsoft.AspNetCore.Mvc;

namespace Triceratops.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
#if (DEBUG)
            return Json(new { Message = "This is the API, and it is running in debug mode!" });
#else
            return Json(new { Message = "This is the API, you probably want to go to the dashboard." });
#endif
        }
    }
}
