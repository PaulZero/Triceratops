using System.IO;
using Triceratops.VolumeManager.Models;

namespace Triceratops.VolumeManager.Services.StorageService.Interfaces
{
    public interface IStorageService
    {
        string[] ListVolumes();

        ServerInstance GetServerDetails(string server);

        Stream GetServerZip(string server);
    }
}
