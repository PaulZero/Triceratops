using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IContainerRepo
    {
        Task<Container> FindByIdAsync(Guid id);

        Task<Container[]> FindByServerIdAsync(Guid stackId);

        Task SaveAsync(Container container);

        Task DeleteAsync(Container container);
    }
}
