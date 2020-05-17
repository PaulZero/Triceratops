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

        public Task<DownloadStream> ReadFileAsync(string relativePath)
        {
            if (IsFile(relativePath))
            {
                var file = GetFileSystemItemFromRelativePath(relativePath) as FileInfo;

                return Task.FromResult(new DownloadStream(File.OpenRead(file.FullName)));
            }

            throw new ArgumentException("The requested relative path does not refer to a file.", relativePath);
        }

        public async Task<bool> WriteFileAsync(string relativePath, Stream stream)
        {
            string tempPath = null;
            var fullPath = $"{VolumeRootDirectory}{relativePath}";

            try
            {
                ValidateRelativePath(relativePath);

                if (File.Exists(fullPath))
                {
                    tempPath = $"{TempDirectory}/{Guid.NewGuid()}.tmp";

                    File.Move(fullPath, tempPath);
                }

                using var writeStream = File.Create(fullPath);

                await stream.CopyToAsync(writeStream);

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to write file to {relativePath}: {exception.Message}");

                if (!File.Exists(fullPath))
                {
                    _logger.LogError("Original file was deleted, attempting to restore.");

                    if (string.IsNullOrWhiteSpace(tempPath))
                    {
                        _logger.LogError("Temp file not set, cannot restore file.");
                    }
                    else if (File.Exists(tempPath))
                    {
                        _logger.LogInformation("Temp file exists, attempting to restore file.");

                        try
                        {
                            File.Move(tempPath, fullPath);

                            _logger.LogInformation("Original file has been restored.");
                        }
                        catch
                        {
                            _logger.LogError($"Could not restore original file - {relativePath} is now missing!");
                        }
                    }
                    else
                    {
                        _logger.LogError("Temp file does not exist, cannot restore file.");
                    }
                }

                return false;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(tempPath) && File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                }
                catch
                {
                    _logger.LogError($"Attempted to clean up temp file {tempPath} but it failed, oops.");
                }
            }
        }

        private void ValidateRelativePath(string relativePath, bool allowTopLevel = false)
        {
            var parts = relativePath.Split('/').Select(p => p.Trim());

            if (!allowTopLevel && parts.Count() == 1)
            {
                throw new Exception("Relative paths cannot exist at the top level of the volume store.");
            }

            if (parts.Contains(".."))
            {
                throw new Exception("Cannot navigate the file system via a relative path.");
            }
        }

        private FileSystemInfo GetFileSystemItemFromRelativePath(string relativePath)
        {
            ValidateRelativePath(relativePath, true);

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
