using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.RouteMapping.Interfaces
{
    public interface IRouteResolver<TRouteEnum>
    {
        IRouteDefinition GetDefinitionForRoute(TRouteEnum route);
    }
}
