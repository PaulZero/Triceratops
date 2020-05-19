using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;

namespace Triceratops.Libraries.RouteMapping.Models
{
    internal class RouteDefinition : IRouteDefinition
    {
        public string Name { get; }

        public string Template { get; }

        public bool IsParameterised => Template.Contains('{');

        public IEnumerable<string> AllowedVerbs { get; }

        protected RouteDefinition(string name, string template, params HttpMethod[] allowedMethods)
        {
            Name = name;
            Template = template;
            AllowedVerbs = allowedMethods.DefaultIfEmpty(HttpMethod.Get).Select(m => m.GetString());
        }

        public string BuildPathWithParameters(object routeData)
            => BuildPathWithParameters(DictionaryHelper.ObjectToDictionary(routeData));

        public string BuildPathWithParameters(IDictionary<string, object> routeData)
        {
            var path = Template;

            foreach (var kvp in routeData)
            {
                path = path.Replace($"{{{kvp.Key}}}", kvp.Value.ToString(), StringComparison.CurrentCultureIgnoreCase);
            }

            if (Name.Contains('{'))
            {
                throw new Exception($"Not all parameters have been bound to route {Name}: {Template}");
            }

            return path;
        }

        public static IRouteDefinition CreatePost(string name, string template)
        {
            return new RouteDefinition(name, template, HttpMethod.Post);
        }

        public static IRouteDefinition CreateGet(string name, string template)
        {
            return new RouteDefinition(name, template, HttpMethod.Get);
        }

        public static IRouteDefinition CreateMixed(string name, string template, params HttpMethod[] methods)
        {
            return new RouteDefinition(name, template, methods);
        }
    }
}
