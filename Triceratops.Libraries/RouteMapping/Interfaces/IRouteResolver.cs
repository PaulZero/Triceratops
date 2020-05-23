namespace Triceratops.Libraries.RouteMapping.Interfaces
{
    public interface IRouteResolver<TRouteEnum>
    {
        IRouteDefinition GetDefinitionForRoute(TRouteEnum route);
    }
}
