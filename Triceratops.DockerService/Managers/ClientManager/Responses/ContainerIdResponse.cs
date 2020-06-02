using Microsoft.Extensions.Logging;
using Triceratops.Libraries.Services;

namespace Triceratops.DockerService.Managers.ClientManager.Responses
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
