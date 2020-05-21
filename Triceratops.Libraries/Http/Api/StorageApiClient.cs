using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models.Storage;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Libraries.Http.Api
{
    internal class StorageApiClient : AbstractHttpClient<StorageApiRoutes>, IStorageApiClient
    {
        public StorageApiClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
        }

        public Task<ServerStorageVolumesResponse> GetServerVolumesAsync(Guid serverId) =>
            Client
            .GetAsync<ServerStorageVolumesResponse>(GetRelativeUrl(StorageApiRoutes.GetServerVolumesById, new { serverId }));

        public Task<ServerStorageVolumesResponse> GetServerVolumesAsync(string serverSlug) =>
            Client
            .GetAsync<ServerStorageVolumesResponse>(GetRelativeUrl(StorageApiRoutes.GetServerVolumesBySlug, new { serverSlug }));

        public Task<DownloadStream> DownloadServerZipAsync(Guid serverId) =>
            Client
            .DownloadAsync(GetRelativeUrl(StorageApiRoutes.DownloadServerZipById, new { serverId }));

        public Task<DownloadStream> DownloadServerZipAsync(string serverSlug) =>
            Client
            .DownloadAsync(GetRelativeUrl(StorageApiRoutes.DownloadServerZipBySlug, new { serverSlug }));

        public Task<DownloadStream> DownloadFileAsync(Guid serverId, string relativeFilePath) =>
            Client
            .DownloadAsync(GetRelativeUrl(StorageApiRoutes.DownloadFileById, new
            {
                serverId,
                fileHash = HashHelper.CreateHash(relativeFilePath)
            }));

        public Task<DownloadStream> DownloadFileAsync(string serverSlug, string relativeFilePath) =>
            Client
            .DownloadAsync(GetRelativeUrl(StorageApiRoutes.DownloadFileBySlug, new
            {
                serverSlug,
                fileHash = HashHelper.CreateHash(relativeFilePath)
            }));

        public async Task<ServerStorageOperationResponse> DeleteFileAsync(Guid serverId, string relativeFilePath)
        {
            try
            {
                await Client.PostAsync(GetRelativeUrl(StorageApiRoutes.DeleteFileById, new
                {
                    serverId,
                    fileHash = HashHelper.CreateHash(relativeFilePath)
                }));

                return ServerStorageOperationResponse.SuccessfulResponse;
            }
            catch
            {
                return ServerStorageOperationResponse.FailedResponse;
            }
            
        }

        public async Task<ServerStorageOperationResponse> DeleteFileAsync(string serverSlug, string relativeFilePath)
        {
            try
            {
                await Client.PostAsync<ServerStorageOperationResponse>(GetRelativeUrl(StorageApiRoutes.DeleteFileBySlug, new
                {
                    serverSlug,
                    fileHash = HashHelper.CreateHash(relativeFilePath)
                }));

                return ServerStorageOperationResponse.SuccessfulResponse;
            }
            catch
            {
                return ServerStorageOperationResponse.FailedResponse;
            }
        }


        public async Task<ServerStorageOperationResponse> UploadFileAsync(Guid serverId, string relativeFilePath, Stream stream)
        {
            try
            {
                await Client.UploadAsync<ServerStorageOperationResponse>(GetRelativeUrl(StorageApiRoutes.UploadFileById, new
                {
                    serverId,
                    fileHash = HashHelper.CreateHash(relativeFilePath)
                }), stream);

                return ServerStorageOperationResponse.SuccessfulResponse;
            }
            catch
            {
                return ServerStorageOperationResponse.FailedResponse;
            }
        }


        public async Task<ServerStorageOperationResponse> UploadFileAsync(string serverSlug, string relativeFilePath, Stream stream)
        {
            try
            {
                await Client.UploadAsync<ServerStorageOperationResponse>(GetRelativeUrl(StorageApiRoutes.UploadFileBySlug, new
                {
                    serverSlug,
                    fileHash = HashHelper.CreateHash(relativeFilePath)
                }), stream);
                
                return ServerStorageOperationResponse.SuccessfulResponse;
            }
            catch
            {
                return ServerStorageOperationResponse.FailedResponse;
            }
        }
    }
}
