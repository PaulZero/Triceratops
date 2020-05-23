using System.Collections.Generic;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Models;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal class ServerApiRouteResolver : AbstractRouteResolver<ServerApiRoutes>
    {
        protected override Dictionary<ServerApiRoutes, IRouteDefinition> DefineRoutes()
        {
            return new Dictionary<ServerApiRoutes, IRouteDefinition>()
            {
                [ServerApiRoutes.GetServerList] = RouteDefinition.CreateGet("Servers_GetServerList", "/servers/list"),
                [ServerApiRoutes.GetServerById] = RouteDefinition.CreateGet("Servers_GetServerById", "/servers/by-id/{serverId}"),
                [ServerApiRoutes.GetServerBySlug] = RouteDefinition.CreateGet("Servers_GetServerBySlug", "/servers/by-slug/{slug}"),
                [ServerApiRoutes.GetServerLogs] = RouteDefinition.CreateGet("Servers_GetServerLogs", "/servers/{serverId}/logs"),

                [ServerApiRoutes.CreateServer] = RouteDefinition.CreatePost("Servers_CreateServer", "/servers/create"),
                [ServerApiRoutes.StartServer] = RouteDefinition.CreatePost("Servers_StartServer", "/servers/{serverId}/start"),
                [ServerApiRoutes.StopServer] = RouteDefinition.CreatePost("Servers_StopServer", "/servers/{serverId}/stop"),
                [ServerApiRoutes.RestartServer] = RouteDefinition.CreatePost("Servers_RestartServer", "/servers/{serverId}/restart"),
                [ServerApiRoutes.DeleteServer] = RouteDefinition.CreatePost("Servers_DeleteServer", "/servers/{serverId}/delete")
            };
        }
    }
}
