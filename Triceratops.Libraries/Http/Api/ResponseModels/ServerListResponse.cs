using System.Collections.Generic;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerListResponse : IApiResponse
    {
        public bool Success { get; set; } = true;

        public string Error { get; set; }

        public IEnumerable<ServerDetailsResponse> Servers { get; set; } = new ServerDetailsResponse[0];
    }
}
