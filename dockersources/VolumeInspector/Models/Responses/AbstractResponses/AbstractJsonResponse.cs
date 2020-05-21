using System.Net;
using System.Text.Json;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;

namespace Triceratops.VolumeInspector.Models.Responses.AbstractResponses
{
    internal abstract class AbstractJsonResponse : IJsonResponse
    {
        public string Json { get; }

        public abstract HttpStatusCode StatusCode { get; }

        public AbstractJsonResponse(VolumeInspectorResponseModel response)
        {
            Json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
