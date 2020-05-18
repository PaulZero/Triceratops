using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Storage;
using Triceratops.VolumeManager.Models;

namespace Triceratops.VolumeManager.Services.StorageService.Interfaces
{
    public interface IStorageService
    {
        string[] GetServerStorageNames();

        ServerInstance GetServerDetails(string server);

        Task<Stream> GetServerZipAsync(string server);

        bool IsFile(string relativePath);

        bool IsDirectory(string relativePath);

        Task<DownloadStream> ReadFileAsync(string relativePath);

        Task<bool> WriteFileAsync(string relativePath, Stream stream);
    }
}
