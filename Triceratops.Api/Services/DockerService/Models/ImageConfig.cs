using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class ImageConfig
    {
        public string Name { get; set; }

        public ImageVersion[] Versions { get; set; }
    }

    public class ImageVersion
    {
        public string Tag { get; set; }

        public Dictionary<string, string> EnvironmentVariables { get; set; }

        [JsonIgnore]
        public bool HasEnvironmentVariables => EnvironmentVariables?.Any() ?? false;
    }
}
