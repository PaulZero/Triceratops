using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Models.StackConfiguration;
using Triceratops.Api.Models.StackConfiguration.Minecraft;

namespace Triceratops.Api.Services.DbService.Memory
{
    public class MemoryDb : IDbService
    {
        public IDbStackRepo StackRepo { get; } = new MemoryStackRepo();

        public class MemoryStackRepo : AbstractMemoryRepo<ContainerStack>, IDbStackRepo
        {
            public MemoryStackRepo()
            {
                Insert(new ContainerStack
                {
                    StackConfigurationType = typeof(MinecraftStackConfiguration)
                });
            }

            public Task<ContainerStack[]> FetchAll()
            {
                return Task.FromResult(Entities);
            }

            public Task<ContainerStack> FetchByIdAsync(uint id)
            {
                var entity = Entities.FirstOrDefault(s => s.Id == id);

                return Task.FromResult(entity);
            }

            public Task<ContainerStack[]> FetchByType<T>() where T : IStackConfiguration
            {
                var results = Entities.Where(s => s.StackConfigurationType == typeof(T)).ToArray();

                return Task.FromResult(results);
            }

            public Task<ContainerStack> Save(ContainerStack stack)
            {
                Insert(stack);

                return Task.FromResult(stack);
            }
        }

        public class AbstractMemoryRepo<T> where T : IDbEntity
        {
            public T[] Entities { get; private set; }

            private readonly Dictionary<uint, T> _items
                = new Dictionary<uint, T>();

            private uint _nextId = 1;

            protected void Insert(params T[] entities)
            {
                foreach (var entity in entities)
                {
                    entity.Id = _nextId++;
                    _items.Add(entity.Id, entity);
                }

                Refresh();
            }

            protected void Delete(uint id)
            {
                InternalDelete(id);

                Refresh();
            }

            protected void Remove(T[] entities)
            {
                foreach (var entity in entities)
                {
                    InternalDelete(entity.Id);
                }

                Refresh();
            }

            private void Refresh()
            {
                Entities = _items.Values.ToArray();
            }

            private void InternalDelete(uint id)
            {
                if (_items.ContainsKey(id))
                {
                    _items.Remove(id);
                }
            }
        }
    }
}
