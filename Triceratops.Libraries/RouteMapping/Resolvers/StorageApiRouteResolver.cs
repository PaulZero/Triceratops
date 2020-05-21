using System.Collections.Generic;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.Libraries.RouteMapping.Interfaces;
using Triceratops.Libraries.RouteMapping.Models;

namespace Triceratops.Libraries.RouteMapping.Resolvers
{
    internal class StorageApiRouteResolver : AbstractRouteResolver<StorageApiRoutes>
    {
        protected override Dictionary<StorageApiRoutes, IRouteDefinition> DefineRoutes()
        {
            return new Dictionary<StorageApiRoutes, IRouteDefinition>()
            {
                [StorageApiRoutes.GetServerVolumesById] = RouteDefinition.CreateGet("Storage_GetServerVolumesById", "/storage/by-id/{serverId}/volumes"),
                [StorageApiRoutes.DownloadServerZipById] = RouteDefinition.CreateGet("Storage_DownloadServerZipById", "/storage/by-id/{serverId}/zip"),
                [StorageApiRoutes.DownloadFileById] = RouteDefinition.CreateGet("Storage_DownloadFileById", "/storage/by-id/{serverId}/download/{fileHash}"),
                [StorageApiRoutes.UploadFileById] = RouteDefinition.CreatePost("Storage_UploadFileById", "/storage/by-id/{serverId}/upload/{fileHash}"),
                [StorageApiRoutes.DeleteFileById] = RouteDefinition.CreatePost("Storage_DeleteFileById", "/storage/by-id/{serverId}/delete/{fileHash}"),

                [StorageApiRoutes.GetServerVolumesBySlug] = RouteDefinition.CreateGet("Storage_GetServerVolumesBySlug", "/storage/by-slug/{serverSlug}/volumes"),
                [StorageApiRoutes.DownloadServerZipBySlug] = RouteDefinition.CreateGet("Storage_DownloadServerZipBySlug", "/storage/by-slug/{serverSlug}/zip"),
                [StorageApiRoutes.DownloadFileBySlug] = RouteDefinition.CreateGet("Storage_DownloadFileBySlug", "/storage/by-slug/{serverSlug}/download/{fileHash}"),
                [StorageApiRoutes.UploadFileBySlug] = RouteDefinition.CreatePost("Storage_UploadFileBySlug", "/storage/by-slug/{serverSlug}/upload/{fileHash}"),
                [StorageApiRoutes.DeleteFileBySlug] = RouteDefinition.CreatePost("Storage_DeleteFileBySlug", "/storage/by-slug/{serverSlug}/delete/{fileHash}"),
            };
        }
    }
}
