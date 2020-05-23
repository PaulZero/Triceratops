using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Api.Services.DockerService.ServiceResponses;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DockerService
{
    public interface IDockerService
    {
        Task<DockerOperationResponse> CreateContainerAsync(Container container);
        Task<DockerOperationResponse> DeleteContainerAsync(Container container);
        Task<ContainerStatusResponse> GetContainerStatusAsync(Container container);
        Task<TemporaryContainerResponse<TemporaryStorageContainer>> GetStorageContainerAsync(Server server);
        Task<DockerOperationResponse> StartContainerAsync(string containerId, bool waitForStart = false);
        Task<DockerOperationResponse> StopContainerAsync(string containerId, bool waitForStop = false);
    }
}