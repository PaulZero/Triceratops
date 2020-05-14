using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Dashboard.Views
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ConditionalRouteLink(this IHtmlHelper helper, bool showLink, string label, string routeName, string elementClass, object routeValues = null)
        {
            if (!showLink)
            {
                return new HtmlString($"<span class=\"button disabled-button\">{label}</span>");
            }

            return helper.RouteLink(label, routeName, routeValues, new Dictionary<string, string>
            {
                ["class"] = elementClass
            });
        }
    }
}
