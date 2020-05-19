using System.Collections.Generic;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Models;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal class DashboardRouteResolver : AbstractRouteResolver<DashboardRoutes>
    {
        protected override Dictionary<DashboardRoutes, IRouteDefinition> DefineRoutes()
        {
            return new Dictionary<DashboardRoutes, IRouteDefinition>()
            {
                [DashboardRoutes.Home] = RouteDefinition.CreateGet("Home", "/"),

                [DashboardRoutes.StartServer] = RouteDefinition.CreateGet("StartServer", "/servers/{slug}/start"),
                [DashboardRoutes.StopServer] = RouteDefinition.CreateGet("StopServer", "/servers/{slug}/stop"),
                [DashboardRoutes.RestartServer] = RouteDefinition.CreateGet("RestartServer", "/servers/{slug}/delete"),
                [DashboardRoutes.DeleteServer] = RouteDefinition.CreateGet("DeleteServer", "/servers/{slug}/delete"),

                [DashboardRoutes.CreateServer] = RouteDefinition.CreateGet("CreateServer", "/servers/create/{serverType}"),
                [DashboardRoutes.SaveServer] = RouteDefinition.CreatePost("SaveServer", "/servers/save"),

                [DashboardRoutes.ListServers] = RouteDefinition.CreateGet("ListServers", "/servers"),
                [DashboardRoutes.ViewServerDetails] = RouteDefinition.CreateGet("ViewServer", "/servers/{slug}"),
                [DashboardRoutes.ViewServerFile] = RouteDefinition.CreateGet("ViewServerFile", "/servers/files/view/{fileHash}"),
                [DashboardRoutes.EditServerFile] = RouteDefinition.CreateGet("EditServerFile", "/servers/files/edit/{fileHash}"),
                [DashboardRoutes.SaveServerFile] = RouteDefinition.CreatePost("SaveServerFile", "/servers/files/save"),

                [DashboardRoutes.SignalRHub] = RouteDefinition.CreateMixed("SignalRHub", "/websocket/api", HttpMethod.Get, HttpMethod.Post)
            };
        }
    }
}
