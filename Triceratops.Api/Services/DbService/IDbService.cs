using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Models.StackConfiguration;

namespace Triceratops.Api.Services.DbService
{
    public interface IDbService
    {
        IDbStackRepo StackRepo { get; }
    }

    public interface IDbStackRepo
    {
        Task<ContainerStack> FetchByIdAsync(uint id);

        Task<ContainerStack[]> FetchAll();

        Task<ContainerStack[]> FetchByType<T>()
            where T : IStackConfiguration;

        Task<ContainerStack> Save(ContainerStack stack);
    }
}
