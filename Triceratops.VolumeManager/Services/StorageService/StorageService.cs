using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.VolumeManager.Models;
using Triceratops.VolumeManager.Services.StorageService.Interfaces;

namespace Triceratops.VolumeManager.Services.StorageService
{
    public class StorageService : IStorageService
    {
        public const string VolumeRootDirectory = "/app/volumes";

        public const string TempDirectory = "/app/temp";

        public StorageService()
        {
            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }
        }

        public string[] ListVolumes()
        {
            var rootDirectory = GetRootDirectory();
            var volumeDirectories = rootDirectory.GetDirectories();
            var volumes = volumeDirectories.Select(d => d.Name);

            return volumes.ToArray();
        }

        public ServerInstance GetServerDetails(string server)
        {
            var rootDirectory = GetRootDirectory();
            var serverDirectory = rootDirectory.GetDirectories().FirstOrDefault(d => d.Name == server);

            if (serverDirectory == null)
            {
                return null;
            }

            var childDirectories = serverDirectory.GetDirectories();

            return new ServerInstance
            { 
                Name = server,
                Directories = serverDirectory.GetDirectories().Select(d => CreateDirectoryTree(d)).ToArray()
            };
        }

        public Stream GetServerZip(string server)
        {
            var rootDirectory = GetRootDirectory();
            var serverDirectory = rootDirectory.GetDirectories().FirstOrDefault(d => d.Name == server);

            if (serverDirectory == null)
            {
                return null;
            }

            var zipFilePath = Path.Combine(TempDirectory, $"{server}.zip");

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(serverDirectory.FullName, zipFilePath);

            var bytes = File.ReadAllBytes(zipFilePath);
            var stream = new MemoryStream(bytes);

            File.Delete(zipFilePath);

            return stream;
        }

        private ServerDirectory CreateDirectoryTree(DirectoryInfo directoryInfo)
        {
            var directory = new ServerDirectory
            {
                Name = directoryInfo.Name,
            };

            directory.Directories = directoryInfo.GetDirectories().Select(d => CreateDirectoryTree(d)).ToArray();
            directory.Files = directoryInfo.GetFiles().Select(f => new ServerFile { Name = f.Name }).ToArray();

            return directory;
        }

        private DirectoryInfo GetRootDirectory()
        {
            var directory = new DirectoryInfo(VolumeRootDirectory);

            if (!directory.Exists)
            {
                directory.Create();
            }

            return directory;
        }
    }
}
