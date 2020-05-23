using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Api.Services.DockerService.Structs;
using Triceratops.Libraries.Helpers;

namespace Triceratops.Api.Services.DockerService.Managers
{
    public class ImageSourceManager
    {
        private const string InternalSourcesPath = "/app/dockersources";

        private readonly List<InternalDockerSource> _internalSources = new List<InternalDockerSource>();

        private readonly ILogger _logger;

        public ImageSourceManager(ILogger logger)
        {
            _logger = logger;

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
                var sourcesDirectory = new DirectoryInfo(InternalSourcesPath);

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

        private InternalDockerSource CreateFromImageConfigFile(FileInfo file)
        {
            try
            {
                var imageConfigContents = File.ReadAllText(file.FullName);
                var imageConfig = JsonHelper.Deserialise<ImageConfig>(imageConfigContents);

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
