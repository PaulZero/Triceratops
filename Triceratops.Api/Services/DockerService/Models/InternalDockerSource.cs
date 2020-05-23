using System.IO;
using System.Linq;
using Triceratops.Api.Services.DockerService.Structs;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class InternalDockerSource
    {
        public DirectoryInfo Directory { get; }

        public ImageConfig Config { get; }

        public InternalDockerSource(DirectoryInfo directory, ImageConfig config)
        {
            Directory = directory;
            Config = config;
        }

        public bool Matches(DockerImageId imageId)
        {
            return Config.Name == imageId.Name && Config.Versions.Any(v => v.Tag == imageId.Tag);
        }
    }
}
