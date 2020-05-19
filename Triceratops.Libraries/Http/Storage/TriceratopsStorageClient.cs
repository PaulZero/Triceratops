using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;
using Triceratops.Libraries.Http.Storage.ResponseModels;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Libraries.Http.Clients.Storage
{
    public class TriceratopsStorageClient : AbstractHttpClient<VolumeManagerRoutes>, ITriceratopsStorageClient
    {
        public TriceratopsStorageClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
            Client.SetBaseUrl(Constants.InternalVolumeManagerUrl);
        }

        public Task<FileDownloadResponse> DownloadFileAsync(string relativeFilePath)
            => Client.DownloadAsync(GetRelativeUrl(VolumeManagerRoutes.DownloadFile, new { fileHash = HashHelper.CreateHash(relativeFilePath) }));

        public Task UploadFileAsync(string relativeFilePath, Stream stream)
            => Client.UploadAsync(GetRelativeUrl(VolumeManagerRoutes.UploadFile, new { fileHash = HashHelper.CreateHash(relativeFilePath) }), stream);

        public Task<ServerStorageResponse> GetServerAsync(string serverSlug)
            => Client.GetAsync<ServerStorageResponse>(GetRelativeUrl(VolumeManagerRoutes.GetServer, new { serverSlug }));

        public Task<ServerStorageNamesResponse> GetServerNamesAsync()
            => Client.GetAsync<ServerStorageNamesResponse>(GetRelativeUrl(VolumeManagerRoutes.GetServerNames));
    }
}
