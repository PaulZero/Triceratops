using System.IO;
using Triceratops.Libraries.Models.Storage;
using Triceratops.VolumeManager.Models;

namespace Triceratops.VolumeManager.Services.StorageService.Interfaces
{
    public interface IStorageService
    {
        string[] ListVolumes();

        ServerInstance GetServerDetails(string server);

        Stream GetServerZip(string server);

        bool IsFile(string relativePath);

        bool IsDirectory(string relativePath);

        DownloadStream GetFileStream(string relativePath);
    }
}
