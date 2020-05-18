using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Http.Storage.Interfaces.Client;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Libraries.Http.Clients.Storage
{
    public class TriceratopsStorageClient : AbstractHttpClient, ITriceratopsStorageClient
    {
        public TriceratopsStorageClient(IPlatformHttpClient httpClient) : base(httpClient)
        {
            Client.SetBaseUrl(TriceratopsConstants.InternalVolumeManagerUrl);
        }

        public Task<FileDownloadResponse> DownloadFileAsync(string relativeFilePath)
            => Client.DownloadAsync($"/servers/files/download/{HashHelper.CreateHash(relativeFilePath)}");

        public Task UploadFileAsync(string relativeFilePath, Stream stream)
            => Client.UploadAsync($"/servers/files/upload/{HashHelper.CreateHash(relativeFilePath)}", stream);

        public Task<ServerStorageResponse> GetServerAsync(string serverName)
            => Client.GetAsync<ServerStorageResponse>($"/servers/{serverName}");

        public Task<ServerStorageNamesResponse> GetServerNamesAsync()
            => Client.GetAsync<ServerStorageNamesResponse>("/servers");
    }
}
