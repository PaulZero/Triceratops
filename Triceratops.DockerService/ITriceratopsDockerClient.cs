using System.Threading.Tasks;
using Triceratops.DockerService.Models;
using Triceratops.DockerService.ServiceResponses;
using Triceratops.Libraries.Models;

namespace Triceratops.DockerService
{
    public interface ITriceratopsDockerClient
    {
        Task<DockerOperationResponse> CreateContainerAsync(Container container);

        Task<DockerOperationResponse> DeleteContainerAsync(Container container);

        Task<DockerStreamResponse> GetContainerLogStreamAsync(string containerId);

        Task<ContainerStatusResponse> GetContainerStatusAsync(Container container);

        Task<TemporaryContainerResponse<TemporaryStorageContainer>> GetStorageContainerAsync(Server server);

        Task<DockerOperationResponse> StartContainerAsync(string containerId, bool waitForStart = false);

        Task<DockerOperationResponse> StopContainerAsync(string containerId, bool waitForStop = false);        
    }
}