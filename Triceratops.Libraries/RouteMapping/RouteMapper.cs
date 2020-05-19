using System;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Resolvers;

namespace Triceratops.Libraries.RouteMapping
{
    public static class RouteMapper
    {
        public static IRouteResolver<ApiRoutes> Api { get; } = new ApiRouteResolver();

        public static IRouteResolver<DashboardRoutes> Dashboard { get; } = new DashboardRouteResolver();

        public static IRouteResolver<VolumeManagerRoutes> VolumeManager { get; } = new VolumeManagerRouteResolver();

        public static IRouteResolver<TRouteEnum> GetResolverForEnum<TRouteEnum>()
        {
            var type = typeof(TRouteEnum);

            if (type == typeof(ApiRoutes))
            {
                return Api as IRouteResolver<TRouteEnum>;
            }

            if (type == typeof(DashboardRoutes))
            {
                return Dashboard as IRouteResolver<TRouteEnum>;
            }

            if (type == typeof(VolumeManagerRoutes))
            {
                return VolumeManager as IRouteResolver<TRouteEnum>;
            }

            throw new Exception("You can't just make up resolvers...");
        }
    }
}
