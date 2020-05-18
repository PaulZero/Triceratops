using Microsoft.Extensions.Logging;

namespace Triceratops.Libraries.Http.Core
{
    public abstract class AbstractHttpClient
    {
        protected IPlatformHttpClient Client { get; }

        public AbstractHttpClient(IPlatformHttpClient httpClient)
        {
            Client = httpClient;
        }
    }
}
