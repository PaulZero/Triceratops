using Microsoft.Extensions.Logging;

namespace Triceratops.Api.Services.DockerService.Managers.ClientManager.Responses
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
