using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Models.StackConfiguration;

namespace Triceratops.Api.Services.DbService
{
    public interface IDbService
    {
        IDbContainerRepo Containers { get; }

        IDbStackRepo Stacks { get; }
    }

    public interface IDbRepo<T>
        where T : IDbEntity
    {
        Task<T> SaveAsync(T entity);

        Task<T[]> SaveAsync(params T[] entities);

        Task DeleteAsync(params T[] entities);
    }

    public interface IDbStackRepo : IDbRepo<ContainerStack>
    {
        Task<ContainerStack> FetchByIdAsync(uint id);

        Task<ContainerStack[]> FetchAllAsync();

        Task<ContainerStack[]> FetchByTypeAsync<T>()
            where T : IStackConfiguration;

        Task<ContainerStack> Save(ContainerStack stack);
    }

    public interface IDbContainerRepo : IDbRepo<Container>
    {
        Task<Container> FetchByIdAsync(uint id);

        Task<Container[]> FetchByStackId(uint stackId);
    }
}
