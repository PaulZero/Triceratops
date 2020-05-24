using Triceratops.Libraries.Http.Api.Models;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerDetailsResponse : AbstractEndpointResponse
    {
        public ServerExtendedDetails Server { get; set; }
    }
}
