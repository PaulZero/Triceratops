using Microsoft.Extensions.Logging;
using Triceratops.Libraries.Http.Api.Interfaces.Client;

namespace Triceratops.Libraries.Http.Api
{
    public class TriceratopsApiClient : ITriceratopsApiClient
    {
        public ITriceratopsServersApiClient Servers { get; }

        public TriceratopsApiClient(ILoggerFactory loggerFactory)
        {
            Servers = new TriceratopsServersApiClient(Triceratops.ApiUrl, loggerFactory.CreateLogger<ITriceratopsServersApiClient>());
        }
    }
}
