using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Libraries.Http.Api.Interfaces.Client
{
    public interface IStorageApiClient
    {
        Task<DownloadStream> DownloadFileAsync(Guid serverId, string relativeFilePath);

        Task<DownloadStream> DownloadFileAsync(string serverSlug, string relativeFilePath);

        Task<DownloadStream> DownloadServerZipAsync(Guid serverId);

        Task<DownloadStream> DownloadServerZipAsync(string serverSlug);

        Task<ServerStorageVolumesResponse> GetServerVolumesAsync(Guid serverId);

        Task<ServerStorageVolumesResponse> GetServerVolumesAsync(string serverSlug);

        Task<ServerStorageOperationResponse> UploadFileAsync(Guid serverId, string relativeFilePath, Stream stream);

        Task<ServerStorageOperationResponse> UploadFileAsync(string serverSlug, string relativeFilePath, Stream stream);

        Task<ServerStorageOperationResponse> DeleteFileAsync(Guid serverId, string relativeFilePath);

        Task<ServerStorageOperationResponse> DeleteFileAsync(string serverSlug, string relativeFilePath);
    }
}
