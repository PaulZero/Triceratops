using Microsoft.Extensions.Logging;
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
        public TriceratopsStorageClient(ILogger logger) : base(Triceratops.VolumeManagerUrl, logger)
        {
        }

        public Task<FileDownloadResponse> DownloadFileAsync(string relativeFilePath)
            => DownloadAsync($"/servers/files/download/{HashHelper.CreateHash(relativeFilePath)}");

        public Task UploadFileAsync(string relativeFilePath, Stream stream)
            => UploadAsync($"/servers/files/upload/{HashHelper.CreateHash(relativeFilePath)}", stream);

        public Task<ServerStorageResponse> GetServerAsync(string serverName)
            => GetAsync<ServerStorageResponse>($"/servers/{serverName}");

        public Task<ServerStorageNamesResponse> GetServerNamesAsync()
            => GetAsync<ServerStorageNamesResponse>("/servers");
    }
}
