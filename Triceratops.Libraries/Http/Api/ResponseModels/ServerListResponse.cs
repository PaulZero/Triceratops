using System.Collections.Generic;
using Triceratops.Libraries.Http.Api.Models;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerListResponse : AbstractEndpointResponse
    {
        public IEnumerable<ServerBasicDetails> Servers { get; set; } = new ServerBasicDetails[0];
    }
}
