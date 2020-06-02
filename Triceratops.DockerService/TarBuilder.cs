using ICSharpCode.SharpZipLib.Tar;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Triceratops.DockerService
{
    public class TarBuilder
    {
        private string _routePath;

        public async Task<MemoryStream> BuildFromDirectory(IDirectoryInfo directory)
        {
            _routePath = directory.FullName;

            var tarball = new MemoryStream();
            using var archive = new TarOutputStream(tarball)
            {
                IsStreamOwner = false
            };

            await AddDirectoryFilesToTar(archive, directory);

            archive.Close();

            tarball.Position = 0;

            return tarball;
        }

        private string GetRelativePath(IFileSystemInfo item)
        {
            var relativePath = item.FullName.Substring(_routePath.Length);

            if (relativePath == "/")
            {
                return string.Empty;
            }

            return relativePath;
        }

        private async Task AddDirectoryFilesToTar(TarOutputStream tarArchive, IDirectoryInfo directory)
        {
            if (!string.IsNullOrWhiteSpace(GetRelativePath(directory)))
            {
                var tarEntry = TarEntry.CreateEntryFromFile(directory.FullName);
                tarEntry.Name = GetRelativePath(directory);
                tarArchive.PutNextEntry(tarEntry);
            }

            // Write each file to the tar.

            foreach (var file in directory.GetFiles())
            {
                var tarEntry = TarEntry.CreateEntryFromFile(file.FullName);
                tarEntry.Name = GetRelativePath(file);
                tarEntry.Size = file.Length;

                tarArchive.PutNextEntry(tarEntry);

                using var fileStream = file.OpenRead();
                await fileStream.CopyToAsync(tarArchive);

                tarArchive.CloseEntry();
            }

            foreach (var childDirectory in directory.GetDirectories())
            {
                await AddDirectoryFilesToTar(tarArchive, childDirectory);
            }
        }
    }
}
