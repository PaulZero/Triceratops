using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class ContainerDetails
    {
        public bool IsRunning { get; }

        public bool IsPaused { get; }

        public bool IsRestarting { get; }

        public string Status { get; }

        public DateTime Created { get; }

        public static ContainerDetails ContainerMissing { get; } = new ContainerDetails("Container does not exist");

        public static ContainerDetails ErrorInspectingContainer { get; } = new ContainerDetails("Error inspecting container");

        public ContainerDetails(ContainerInspectResponse containerInspectResponse)
        {
            IsRunning = containerInspectResponse.State.Running;
            IsPaused = containerInspectResponse.State.Paused;
            IsRestarting = containerInspectResponse.State.Restarting;
            Status = containerInspectResponse.State.Status;
            Created = containerInspectResponse.Created;
        }

        protected ContainerDetails(string status)
        {
            Status = status;
        }
    }
}
