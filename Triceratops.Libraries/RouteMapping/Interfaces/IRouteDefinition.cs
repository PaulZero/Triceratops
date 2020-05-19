using System.Collections.Generic;

namespace Triceratops.Libraries.RouteMapping.Interfaces
{
    public interface IRouteDefinition
    {
        string Name { get; }

        string Template { get; }

        bool IsParameterised { get; }

        IEnumerable<string> AllowedVerbs { get; }

        string BuildPathWithParameters(object routeData);

        string BuildPathWithParameters(IDictionary<string, object> routeData);
    }
}
