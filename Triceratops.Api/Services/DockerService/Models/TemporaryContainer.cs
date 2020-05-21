using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class TemporaryContainer
    {
        public string DockerContainerId { get; }

        public Guid TemporaryContainerId { get; }

        public DateTime LastAccessed { get; private set; }

        public TemporaryContainer(string dockerContainerId, Guid temporaryContainerId)
        {
            DockerContainerId = dockerContainerId;
            TemporaryContainerId = temporaryContainerId;
        }

        public void RefreshLastAccessed()
        {
            LastAccessed = DateTime.Now;
        }

        public void Destroy(DockerClient dockerClient)
        {
            dockerClient.Containers.RemoveContainerAsync(DockerContainerId, new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = false
            }).Wait();
        }
    }
}
