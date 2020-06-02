using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Triceratops.DockerService.Models;
using Triceratops.DockerService.Structs;
using Triceratops.Libraries.Helpers;

namespace Triceratops.DockerService.Managers
{
    public class ImageSourceManager
    {
        public const string InternalSourcesPath = "/app/dockersources";

        private readonly List<InternalDockerSource> _internalSources = new List<InternalDockerSource>();

        private readonly ILogger _logger;

        private readonly IFileSystem _fileSystem;

        public ImageSourceManager(ILogger logger, IFileSystem fileSystem)
        {
            _logger = logger;
            _fileSystem = fileSystem;

            RefreshInternalSources();
        }

        public InternalDockerSource Get(DockerImageId imageId)
        {
            return Find(imageId);
        }

        public bool IsInternalImage(DockerImageId imageId)
        {
            return Find(imageId) != null;
        }

        public void RefreshInternalSources()
        {
            try
            {
                var sourcesDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(InternalSourcesPath);

                _internalSources.Clear();

                foreach (var directory in sourcesDirectory.GetDirectories())
                {
                    var files = directory.GetFiles();
                    if (files.Any(f => f.Name == "ImageConfig.json") && files.Any(f => f.Name == "Dockerfile"))
                    {
                        var source = CreateFromImageConfigFile(files.First(f => f.Name == "ImageConfig.json"));

                        if (source != null)
                        {
                            _internalSources.Add(source);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to refresh internal Docker sources: {exception.Message}");
            }
        }

        private InternalDockerSource Find(DockerImageId imageId)
        {
            try
            {
                return _internalSources.FirstOrDefault(s => s.Matches(imageId));
            }
            catch
            {
                return null;
            }
        }

        private InternalDockerSource CreateFromImageConfigFile(IFileInfo file)
        {
            try
            {
                using var readStream = file.OpenRead();
                using var reader = new StreamReader(readStream);
                var fileContents = reader.ReadToEnd();

                var imageConfig = JsonHelper.Deserialise<ImageConfig>(fileContents);

                return new InternalDockerSource(file.Directory, imageConfig);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Unable to read internal Docker source from image config at '{file?.FullName}': {exception.Message}");

                return null;
            }
        }
    }
}
