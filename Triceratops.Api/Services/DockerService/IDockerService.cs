using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DockerService.Models;

namespace Triceratops.Api.Services.DockerService
{
    /// <summary>
    /// The Docker service is just a wrapper around Docker.
    /// </summary>
    public interface IDockerService
    {
        Task<bool> CreateContainerAsync(Container container);

        Task<ContainerDetails> GetContainerStatusAsync(Container container);

        Task<string> CreateContainerAsync(string imageName, string imageVersion, string containerName, IEnumerable<string> env = default);

        Task RunContainerAsync(string containerId, params string[] parameters);

        Task StopContainerAsync(string containerId);

        Task DeleteContainerAsync(string containerId, bool force = false);
    }
}
