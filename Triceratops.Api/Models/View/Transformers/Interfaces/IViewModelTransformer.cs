using System.Collections.Generic;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.View;

namespace Triceratops.Api.Models.View.Transformers.Interfaces
{
    public interface IViewModelTransformer
    {
        Task<ContainerViewModel> WrapContainerAsync(Container container);

        Task<List<ContainerViewModel>> WrapContainersAsync(IEnumerable<Container> containers);

        Task<ServerViewModel> WrapServerAsync(Server server);

        Task<List<ServerViewModel>> WrapServersAsync(IEnumerable<Server> servers);
    }
}