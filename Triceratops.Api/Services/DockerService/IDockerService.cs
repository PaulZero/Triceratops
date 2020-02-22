using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService
{
    public interface IDockerService
    {
        Task<string> CreateContainer(string imageName, string containerName);

        Task RunContainer(string containerId, params string[] parameters);

        Task StopContainer(string containerId);

        Task DeleteContainer(string containerId, bool force = false);

        Task DownloadImage(string imageName, string version = "latest");
    }
}
