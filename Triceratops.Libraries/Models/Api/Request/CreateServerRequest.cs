using Newtonsoft.Json;
using System;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Models.Api.Request
{
    public class CreateServerRequest
    {
        public string ConfigurationTypeName { get; set; }

        public string JsonConfiguration { get; set; }

        [JsonIgnore]
        public Type ConfigurationType
        {
            get => Type.GetType(ConfigurationTypeName);
            set => ConfigurationTypeName = value.FullName;
        }

        public CreateServerRequest()
        {
        }

        public CreateServerRequest(AbstractServerConfiguration configuration)
        {
            JsonConfiguration = JsonConvert.SerializeObject(configuration);
            ConfigurationType = configuration.GetType();
        }
    }
}
