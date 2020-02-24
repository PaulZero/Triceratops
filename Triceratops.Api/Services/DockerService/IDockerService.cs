using System.Collections.Generic;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService
{
    public interface IDockerService
    {
        Task<string> CreateContainerAsync(string imageName, string containerName, IEnumerable<string> env = default);

        Task RunContainerAsync(string containerId, params string[] parameters);

        Task StopContainerAsync(string containerId);

        Task DeleteContainerAsync(string containerId, bool force = false);

        Task DownloadImageAsync(string imageName, string version = "latest");
    }
}
