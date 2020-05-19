using System;
using System.Collections.Generic;
using System.Text;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Models;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal class ApiRouteResolver : AbstractRouteResolver<ApiRoutes>
    {
        protected override Dictionary<ApiRoutes, IRouteDefinition> DefineRoutes()
        {
            return new Dictionary<ApiRoutes, IRouteDefinition>()
            {
                [ApiRoutes.GetServerList] = RouteDefinition.CreateGet("GetServerList", "/servers/list"),
                [ApiRoutes.GetServerById] = RouteDefinition.CreateGet("GetServerById", "/servers/by-id/{serverId}"),
                [ApiRoutes.GetServerBySlug] = RouteDefinition.CreateGet("GetServerBySlug", "/servers/by-slug/{slug}"),
                [ApiRoutes.GetServerLogs] = RouteDefinition.CreateGet("GetServerLogs", "/servers/{serverId}/logs"),

                [ApiRoutes.CreateServer] = RouteDefinition.CreatePost("CreateServer", "/servers/create"),
                [ApiRoutes.StartServer] = RouteDefinition.CreatePost("StartServer", "/servers/{serverId}/start"),
                [ApiRoutes.StopServer] = RouteDefinition.CreatePost("StopServer", "/servers/{serverId}/stop"),
                [ApiRoutes.RestartServer] = RouteDefinition.CreatePost("RestartServer", "/servers/{serverId}/restart"),
                [ApiRoutes.DeleteServer] = RouteDefinition.CreatePost("DeleteServer", "/servers/{serverId}/delete")
            };
        }
    }
}
