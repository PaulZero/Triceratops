using Microsoft.AspNetCore.Mvc;
using Triceratops.Libraries.RouteMapping;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Dashboard.Controllers
{
    public abstract class AbstractDashboardController : Controller
    {
        protected IActionResult RedirectToRoute(DashboardRoutes route)
        {
            return RedirectToRoute(GetRouteName(route));
        }

        protected IActionResult RedirectToRoute(DashboardRoutes route, object routeValues)
        {
            return RedirectToRoute(GetRouteName(route), routeValues);
        }

        private string GetRouteName(DashboardRoutes route)
        {
            return RouteMapper.Dashboard.GetDefinitionForRoute(route).Name;
        }
    }
}
