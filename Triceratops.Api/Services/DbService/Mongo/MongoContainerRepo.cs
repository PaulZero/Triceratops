using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoContainerRepo : IContainerRepo
    {
        private readonly IDbService _dbService;

        private readonly IMongoCollection<Container> _mongoCollection;

        public MongoContainerRepo(IDbService dbService, IMongoCollection<Container> mongoCollection)
        {
            _dbService = dbService;
            _mongoCollection = mongoCollection;
        }

        public async Task DeleteAsync(Container container)
        {            
            await _mongoCollection.DeleteOneAsync(CreateFindByIdFilter(container.Id));

            foreach (var volume in container.Volumes)
            {
                await _dbService.Volumes.DeleteAsync(volume);
            }
        }

        public async Task<Container> FindByIdAsync(Guid id)
        {
            var result = await _mongoCollection.FindAsync(CreateFindByIdFilter(id));

            return await result.FirstOrDefaultAsync();
        }

        public async Task<Container[]> FindByServerIdAsync(Guid serverId)
        {
            var result = await _mongoCollection.FindAsync(CreateFindByServerIdFilter(serverId));
            var containers = await result.ToListAsync();

            return containers.ToArray();
        }

        public async Task SaveAsync(Container container)
        {
            foreach (var volume in container.Volumes)
            {
                if (volume.ContainerId == default)
                {
                    volume.ContainerId = container.Id;
                }
            }

            await _mongoCollection.ReplaceOneAsync(
                CreateFindByIdFilter(container.Id),
                container,
                new ReplaceOptions { IsUpsert = true }
            );

            foreach (var volume in container.Volumes)
            {
                await _dbService.Volumes.SaveAsync(volume);
            }
        }

        private FilterDefinition<Container> CreateFindByIdFilter(Guid id)
        {
            return Builders<Container>.Filter.Where(c => c.Id == id);
        }

        private FilterDefinition<Container> CreateFindByServerIdFilter(Guid serverId)
        {
            return Builders<Container>.Filter.Where(c => c.ServerId == serverId);
        }
    }
}
