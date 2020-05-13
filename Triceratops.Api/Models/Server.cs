using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Enums;

namespace Triceratops.Api.Models
{
    public class Server
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string JsonConfiguration { get; set; }

        public string ConfigurationTypeName { get; set; }

        [JsonIgnore]
        public Type ConfigurationType
        {
            get => Type.GetType(ConfigurationTypeName);
            set => ConfigurationTypeName = value.FullName;
        }
        public List<Container> Containers { get; set; } = new List<Container>();

        public ushort[] Ports => Containers.Select(c => c.Port).ToArray();

        public object DeserialiseConfiguration()
        {
            return JsonConvert.DeserializeObject(JsonConfiguration, ConfigurationType);
        }

        public void SetConfiguration(object configuration)
        {
            ConfigurationType = configuration.GetType();
            JsonConfiguration = JsonConvert.SerializeObject(configuration);
        }
    }
}
