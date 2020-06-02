using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.DockerService
{
    internal interface IStorageContainer
    {
        Task<bool> CheckContainerAliveAsync();

        Task<IEnumerable<VolumeDirectory>> GetServerStorageAsync();

        Task<DownloadStream> DownloadServerZipAsync();

        Task<DownloadStream> DownloadFileAsync(string relativeFilePath);

        Task UploadFileAsync(string relativeFilePath, Stream fileStream);

        Task DeleteFileAsync(string relativeFilePath);
    }
}
