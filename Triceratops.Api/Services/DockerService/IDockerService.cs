using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DockerService
{
    /// <summary>
    /// The Docker service is just a wrapper around Docker.
    /// </summary>
    public interface IDockerService
    {
        Task<bool> CreateContainerAsync(Container container);

        Task<ContainerDetails> GetContainerStatusAsync(Container container);

        Task RunContainerAsync(string containerId, params string[] parameters);

        Task StopContainerAsync(string containerId);

        Task DeleteContainerAsync(string containerId, bool force = false);

        Task UpdateVolumeServerAsync(Server[] servers);

        Task<string[]> GetContainerLogAsync(string containerId, uint rows = 300);
    }
}
