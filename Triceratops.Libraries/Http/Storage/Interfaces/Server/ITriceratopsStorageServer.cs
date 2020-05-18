using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Storage.ResponseModels;

namespace Triceratops.Libraries.Http.Storage.Interfaces.Server
{
    public interface ITriceratopsStorageServer
    {
        Task<ServerStorageNamesResponse> GetServerNamesAsync();

        Task<ServerStorageResponse> GetServerAsync(string serverSlug);

        Task<IActionResult> DownloadFileAsync(string fileHash);

        Task<bool> UploadFileAsync(string fileHash);

        Task<IActionResult> DownloadServerZip(string serverSlug);
    }
}
