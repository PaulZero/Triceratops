using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models.Persistence.Stacks;
using Triceratops.Api.Models.StackConfiguration;
using Triceratops.Api.Models.StackConfiguration.Minecraft;
using Triceratops.Api.Models.StackConfiguration.MinecraftBedrock;

namespace Triceratops.Api.Services.DbService.Memory
{
    public class MemoryDb : IDbService
    {
        public IDbContainerRepo Containers { get; } = new MemoryContainerRepo();

        public IDbStackRepo Stacks { get; } = new MemoryStackRepo();

        public MemoryDb()
        {
            var stackRecord = new ContainerStack
            {
                StackType = typeof(MinecraftBedrockStack)
            };

            stackRecord.SetConfig(new MinecraftBedrockStackConfig());

            Stacks.SaveAsync(stackRecord).Wait();

            Containers.SaveAsync(new Container
            {
                StackId = stackRecord.Id,                
                ImageName = "itzg/minecraft-bedrock-server"
            });
        }
    }


    public class MemoryStackRepo : AbstractMemoryRepo<ContainerStack>, IDbStackRepo
    {
        public Task<ContainerStack> SaveAsync(ContainerStack entity)
        {
            InsertEntities(entity);

            return Task.FromResult(entity);
        }

        public Task<ContainerStack[]> SaveAsync(params ContainerStack[] entities)
        {
            InsertEntities(entities);

            return Task.FromResult(entities);
        }

        public Task DeleteAsync(params ContainerStack[] entities)
        {
            DeleteEntities(entities);

            return Task.CompletedTask;
        }

        public Task<ContainerStack[]> FetchAllAsync()
        {
            return Task.FromResult(Entities);
        }

        public Task<ContainerStack> FetchByIdAsync(uint id)
        {
            var entity = Entities.FirstOrDefault(s => s.Id == id);

            return Task.FromResult(entity);
        }

        public Task<ContainerStack[]> FetchByTypeAsync<T>() where T : IStack
        {
            var results = Entities.Where(s => s.StackType == typeof(T)).ToArray();

            return Task.FromResult(results);
        }

        public Task<ContainerStack> Save(ContainerStack stack)
        {
            InsertEntities(stack);

            return Task.FromResult(stack);
        }
    }

    public class MemoryContainerRepo : AbstractMemoryRepo<Container>, IDbContainerRepo
    {
        public Task<Container> SaveAsync(Container entity)
        {
            InsertEntities(entity);

            return Task.FromResult(entity);
        }
        public Task<Container[]> SaveAsync(params Container[] entities)
        {
            InsertEntities(entities);

            return Task.FromResult(entities);
        }

        public Task DeleteAsync(params Container[] entities)
        {
            DeleteEntities(entities);

            return Task.CompletedTask;
        }

        public Task<Container> FetchByIdAsync(uint id)
        {
            var entity = Entities.FirstOrDefault(c => c.Id == id);

            return Task.FromResult(entity);
        }

        public Task<Container[]> FetchByStackId(uint stackId)
        {
            var entities = Entities
                .Where(c => c.StackId == stackId)
                .ToArray();

            return Task.FromResult(entities);
        }
    }

    public class AbstractMemoryRepo<T> where T : IDbEntity
    {
        public T[] Entities { get; private set; }

        private readonly Dictionary<uint, T> _items
            = new Dictionary<uint, T>();

        private uint _nextId = 1;

        protected void InsertEntities(params T[] entities)
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

        protected void DeleteEntities(T[] entities)
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