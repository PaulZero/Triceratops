using System;
using System.Text.Json.Serialization;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Http.Api.RequestModels
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
            JsonConfiguration = JsonHelper.Serialise(configuration);
            ConfigurationType = configuration.GetType();
        }
    }
}
