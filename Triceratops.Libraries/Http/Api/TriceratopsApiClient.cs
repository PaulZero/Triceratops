using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Core;

namespace Triceratops.Libraries.Http.Api
{
    public class TriceratopsApiClient : ITriceratopsApiClient
    {
        public IServerApiClient Servers { get; }

        public IStorageApiClient Storage { get; }

        public TriceratopsApiClient(IPlatformHttpClient httpClient)
        {
            httpClient.SetBaseUrl(Constants.InternalApiUrl);

            Servers = new ServerApiClient(httpClient);
            Storage = new StorageApiClient(httpClient);
        }
    }
}
