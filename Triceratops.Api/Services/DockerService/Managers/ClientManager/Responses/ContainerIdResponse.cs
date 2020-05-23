using Microsoft.Extensions.Logging;

namespace Triceratops.Api.Services.DockerService.Managers.ClientManager.Responses
{
    public class ContainerIdResponse : LoggedResponse<ContainerIdResponse>
    {
        public string ContainerId { get; private set; }

        public ContainerIdResponse(ILogger logger) : base(logger)
        {
        }

        public void WithContainerId(string containerId)
        {
            ContainerId = containerId;

            IsSuccessful();
        }
    }
}
