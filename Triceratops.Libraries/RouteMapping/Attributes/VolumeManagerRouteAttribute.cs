using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Libraries.RouteMapping.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class VolumeManagerRouteAttribute : Attribute, IRouteTemplateProvider, IActionHttpMethodProvider
    {
        public IEnumerable<string> HttpMethods { get; }

        public string Template { get; }

        public int? Order { get; }

        public string Name { get; }

        public VolumeManagerRouteAttribute(VolumeManagerRoutes route)
        {
            var template = RouteMapper.VolumeManager.GetDefinitionForRoute(route);

            HttpMethods = template.AllowedVerbs;
            Template = template.Template;
            Name = template.Name;
        }
    }
}
