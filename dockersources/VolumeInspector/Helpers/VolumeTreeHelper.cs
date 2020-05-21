using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Triceratops.VolumeInspector._Libraries;

namespace Triceratops.VolumeInspector.Helpers
{
    internal static class VolumeTreeHelper
    {
        public static Stream CreateZip()
        {
            var rootDirectory = new DirectoryInfo(VolumeInspectorConstants.VolumesPath);

            var zipStream = new MemoryStream();

            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

            AddDirectoryToZipArchive(rootDirectory, archive);

            return zipStream;
        }

        public static IEnumerable<VolumeDirectory> BuildAll()
        {
            var rootDirectory = new DirectoryInfo(VolumeInspectorConstants.VolumesPath);

            return rootDirectory.GetDirectories().Select(Build);
        }

        public static VolumeDirectory Build(DirectoryInfo sourceDirectory)
        {
            var files = sourceDirectory.GetFiles().Select(sourceFile =>
            {
                return new VolumeFile
                {
                    Name = sourceFile.Name,
                    RelativePath = CreateRelativePath(sourceFile)
                };
            });

            var childDirectories = sourceDirectory.GetDirectories().Select(Build);

            return new VolumeDirectory
            {
                Name = sourceDirectory.Name,
                RelativePath = CreateRelativePath(sourceDirectory),
                Files = files,
                Directories = childDirectories
            };
        }

        private static void AddDirectoryToZipArchive(DirectoryInfo directory, ZipArchive zipArchive)
        {
            zipArchive.CreateEntry(CreateRelativePath(directory));

            foreach (var file in directory.GetFiles())
            {
                zipArchive.CreateEntryFromFile(file.FullName, CreateRelativePath(file), CompressionLevel.Fastest);
            }

            foreach (var childDirectory in directory.GetDirectories())
            {
                AddDirectoryToZipArchive(childDirectory, zipArchive);
            }
        }

        private static string CreateRelativePath(FileSystemInfo item)
        {
            return item.FullName.Substring(VolumeInspectorConstants.VolumesPath.Length);
        }
    }
}
