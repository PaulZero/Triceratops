using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.RouteMapping.Interfaces;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal abstract class AbstractRouteResolver<TRouteEnum> : IRouteResolver<TRouteEnum>
    {
        private readonly Dictionary<TRouteEnum, IRouteDefinition> _routeDefinitions;

        public AbstractRouteResolver()
        {
            _routeDefinitions = DefineRoutes();

            ValidateRoutes();
        }

        public IRouteDefinition GetDefinitionForRoute(TRouteEnum route)
        {
            return _routeDefinitions[route];
        }

        protected abstract Dictionary<TRouteEnum, IRouteDefinition> DefineRoutes();

        private void ValidateRoutes()
        {
            var supportedValues = Enum.GetValues(typeof(TRouteEnum)).Cast<TRouteEnum>();

            if (!supportedValues.All(v => _routeDefinitions.ContainsKey(v)))
            {
                throw new Exception($"{GetType().Name} MUST define route definitions for ALL available routes!");
            }
        }

        
    }
}
