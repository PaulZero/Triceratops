using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Http;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Services.VolumeService.Interfaces
{
    public interface IVolumeService
    {
        Task<string[]> GetServerNamesAsync();

        Task<ServerStorage> GetServerAsync(string serverName);

        Task<ReceivedFile> DownloadFileAsync(string relativeFilePath);

        Task UploadFileAsync(string relativeFilePath, Stream stream);
    }
}
