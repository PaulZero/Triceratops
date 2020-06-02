using Microsoft.Extensions.Logging;
using Triceratops.Libraries.Services;

namespace Triceratops.DockerService.Managers.ClientManager.Responses
{
    public class RawDockerResponse<TDockerResponse> : LoggedResponse
        where TDockerResponse : class
    {
        public TDockerResponse DockerResponse { get; private set; }

        public RawDockerResponse(ILogger logger) : base(logger)
        {
        }

        public void WithResponse(TDockerResponse response)
        {
            DockerResponse = response;

            IsSuccessful();
        }
    }
}
