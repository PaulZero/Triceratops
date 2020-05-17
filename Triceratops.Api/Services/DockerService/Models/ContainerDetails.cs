using System;
using Triceratops.Libraries.Enums;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class ContainerDetails
    {
        public DateTime Created { get; }

        public string DockerId { get; }

        public ServerContainerState State { get; }

        public string Status { get; }

        public static ContainerDetails ContainerMissing { get; } = new ContainerDetails("Container does not exist");

        public static ContainerDetails ErrorInspectingContainer { get; } = new ContainerDetails("Error inspecting container");

        public ContainerDetails(string dockerId, string status, DateTime created, ServerContainerState state)
        {
            DockerId = dockerId;
            Status = status;
            Created = created;
            State = state;        
        }

        protected ContainerDetails(string status)
        {
            Status = status;
        }
    }
}
