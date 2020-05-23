using Docker.DotNet.Models;
using System;
using Triceratops.Api.Services.DockerService.Interfaces;
using Triceratops.Libraries.Enums;

namespace Triceratops.Api.Services.DockerService.ServiceResponses
{
    public class ContainerStatusResponse : DockerOperationResponse
    {
        public DateTime Created { get; }

        public ServerContainerState State { get; }

        public ContainerStatusResponse(IReadOnlyServiceResponse response) : base(response)
        {
        }

        public ContainerStatusResponse(
            IReadOnlyServiceResponse response,
            ContainerInspectResponse dockerResponse
        ) : base(response)
        {
            Created = dockerResponse.Created;
            State = CreateFromDockerContainerState(dockerResponse.State);
        }

        private ServerContainerState CreateFromDockerContainerState(ContainerState containerState)
        {
            if (containerState.Running)
            {
                return ServerContainerState.Running;
            }

            return ServerContainerState.Stopped;
        }
    }
}
