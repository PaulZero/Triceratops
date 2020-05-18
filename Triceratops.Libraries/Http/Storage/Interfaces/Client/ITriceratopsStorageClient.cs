using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Libraries.Http.Storage.Interfaces.Client
{
    public interface ITriceratopsStorageClient
    {
        Task<ServerStorageNamesResponse> GetServerNamesAsync();

        Task<ServerStorageResponse> GetServerAsync(string serverName);

        Task<FileDownloadResponse> DownloadFileAsync(string relativeFilePath);

        Task UploadFileAsync(string relativeFilePath, Stream stream);
    }
}
