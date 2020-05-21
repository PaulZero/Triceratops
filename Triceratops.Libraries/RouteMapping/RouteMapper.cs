using System;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Resolvers;

namespace Triceratops.Libraries.RouteMapping
{
    public static class RouteMapper
    {
        public static IRouteResolver<ServerApiRoutes> ServerApi { get; } = new ServerApiRouteResolver();

        public static IRouteResolver<DashboardRoutes> Dashboard { get; } = new DashboardRouteResolver();

        public static IRouteResolver<StorageApiRoutes> StorageApi { get; } = new StorageApiRouteResolver();

        public static IRouteResolver<TRouteEnum> GetResolverForEnum<TRouteEnum>()
        {
            var type = typeof(TRouteEnum);

            if (type == typeof(ServerApiRoutes))
            {
                return ServerApi as IRouteResolver<TRouteEnum>;
            }

            if (type == typeof(DashboardRoutes))
            {
                return Dashboard as IRouteResolver<TRouteEnum>;
            }

            if (type == typeof(StorageApiRoutes))
            {
                return StorageApi as IRouteResolver<TRouteEnum>;
            }

            throw new Exception("You can't just make up resolvers...");
        }
    }
}
