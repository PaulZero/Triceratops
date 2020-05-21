using System;
using Triceratops.Libraries.RouteMapping;
using Triceratops.Libraries.RouteMapping.Interfaces;

namespace Triceratops.Libraries.Http.Core
{
    public abstract class AbstractHttpClient<TRouteEnum>
    {
        protected IPlatformHttpClient Client { get; }

        protected IRouteResolver<TRouteEnum> RouteResolver => RouteMapper.GetResolverForEnum<TRouteEnum>();

        public AbstractHttpClient(IPlatformHttpClient httpClient)
        {
            Client = httpClient;
        }

        protected string GetRelativeUrl(TRouteEnum route, object routeData = null)
        {
            var definition = RouteResolver.GetDefinitionForRoute(route);

            if (definition.IsParameterised)
            {
                if (routeData == null)
                {
                    throw new Exception($"{definition.Name} is a parameterised route, you must specify route parameters");
                }

                return definition.BuildPathWithParameters(routeData);                
            }

            return definition.Template;
        }
    }
}
