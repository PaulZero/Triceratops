using System;
using System.Collections.Generic;
using System.Text;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Models;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal class VolumeManagerRouteResolver : AbstractRouteResolver<VolumeManagerRoutes>
    {
        protected override Dictionary<VolumeManagerRoutes, IRouteDefinition> DefineRoutes()
        {
            return new Dictionary<VolumeManagerRoutes, IRouteDefinition>()
            {
                [VolumeManagerRoutes.GetServerNames] = RouteDefinition.CreateGet("GetServerNames", "/servers"),
                [VolumeManagerRoutes.GetServer] = RouteDefinition.CreateGet("GetServer", "/servers/{serverSlug}"),
                [VolumeManagerRoutes.DownloadServerZip] = RouteDefinition.CreateGet("DownloadServerZip", "/servers/{serverSlug}/zip"),

                [VolumeManagerRoutes.DownloadFile] = RouteDefinition.CreateGet("DownloadFile", "/files/download/{fileHash}"),
                [VolumeManagerRoutes.UploadFile] = RouteDefinition.CreatePost("UploadFile", "/files/upload/{fileHash}")                
            };
        }
    }
}
