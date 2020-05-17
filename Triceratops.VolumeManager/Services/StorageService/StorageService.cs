using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Storage;
using Triceratops.VolumeManager.Models;
using Triceratops.VolumeManager.Services.StorageService.Interfaces;

namespace Triceratops.VolumeManager.Services.StorageService
{
    public class StorageService : IStorageService
    {
        public const string VolumeRootDirectory = "/app/volumes";

        public const string TempDirectory = "/app/temp";

        private readonly ILogger _logger;

        public StorageService(ILogger<IStorageService> logger)
        {
            _logger = logger;

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

        public ServerStorage GetServerDetails(string server)
        {
            var rootDirectory = GetRootDirectory();
            var serverDirectory = rootDirectory.GetDirectories().FirstOrDefault(d => d.Name == server);

            if (serverDirectory == null)
            {
                return null;
            }

            var childDirectories = serverDirectory.GetDirectories();

            return new ServerStorage
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

        public bool IsFile(string relativePath)
        {
            try
            {
                return GetFileSystemItemFromRelativePath(relativePath) is FileInfo;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public bool IsDirectory(string relativePath)
        {
            try
            {
                return GetFileSystemItemFromRelativePath(relativePath) is DirectoryInfo;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public DownloadStream GetFileStream(string relativePath)
        {
            if (IsFile(relativePath))
            {
                var file = GetFileSystemItemFromRelativePath(relativePath) as FileInfo;

                return new DownloadStream(File.OpenRead(file.FullName));
            }

            throw new ArgumentException("The requested relative path does not refer to a file.", relativePath);
        }

        private FileSystemInfo GetFileSystemItemFromRelativePath(string relativePath)
        {
            if (relativePath.Split('/').Contains(".."))
            {
                _logger.LogWarning($"Invalid path was received and chucked out: {relativePath}");

                throw new ArgumentException("No path fuckery is allowed!", nameof(relativePath));
            }

            var fullPath = $"{VolumeRootDirectory}{relativePath}";

            _logger.LogInformation($"Created full path from {relativePath}: {fullPath}");

            if (Directory.Exists(fullPath))
            {
                _logger.LogInformation($"{fullPath} is a directory!");

                return new DirectoryInfo(fullPath);
            }

            if (File.Exists(fullPath))
            {
                _logger.LogInformation($"{fullPath} is a file!");

                return new FileInfo(fullPath);
            }

            _logger.LogInformation($"{fullPath} is not a file or a directory!");

            throw new FileNotFoundException("The requested relative path does not refer to a file or directory.", relativePath);
        }

        private ServerDirectory CreateDirectoryTree(DirectoryInfo directoryInfo)
        {
            var directory = new ServerDirectory
            {
                Name = directoryInfo.Name,
                RelativePath = CreateRelativePath(directoryInfo.FullName)
            };

            directory.Directories = directoryInfo.GetDirectories().Select(d => CreateDirectoryTree(d)).ToArray();
            directory.Files = directoryInfo.GetFiles().Select(f => new ServerFile
            {
                Name = f.Name,
                RelativePath = CreateRelativePath(f.FullName)
            }).ToArray();

            return directory;
        }

        private string CreateRelativePath(string fullPath)
        {
            if (fullPath.Contains(VolumeRootDirectory))
            {
                return fullPath.Remove(0, VolumeRootDirectory.Length);
            }

            return fullPath;
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
