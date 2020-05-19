using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using Triceratops.Libraries.RouteMapping;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Libraries.View.TagHelpers
{
    [HtmlTargetElement("dashboard-link")]
    public class DashboardLinkTagHelper : TagHelper
    {
        public DashboardRoutes? Route { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Route == null)
            {
                throw new Exception("Must specify a route when using a dashboard link element!");
            }

            output.TagName = "a";

            var definition = RouteMapper.Dashboard.GetDefinitionForRoute((DashboardRoutes)Route);

            if (!definition.IsParameterised)
            {
                output.Attributes.SetAttribute("href", definition.Template);

                return;
            }

            var routeParameters = context.AllAttributes
                .Where(a => a.Name.StartsWith("param-"))
                .ToDictionary(a => a.Name.Replace("param-", ""), a => a.Value);

            if (!routeParameters.Any())
            {
                throw new Exception($"{definition.Name} is a parameterised route: {definition.Template}");
            }

            output.Attributes.SetAttribute("href", definition.BuildPathWithParameters(routeParameters));
        }
    }
}
