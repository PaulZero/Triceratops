using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Events;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoVolumeRepo : IVolumeRepo
    {
        private readonly IMongoCollection<Volume> _mongoCollection;

        public MongoVolumeRepo(IMongoCollection<Volume> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<Volume[]> FindAllAsync()
        {
            var result = await _mongoCollection.FindAsync(Builders<Volume>.Filter.Empty);
            var volumes = await result.ToListAsync();

            return volumes.ToArray();
        }

        public async Task<Volume[]> FindByContainerIdAsync(Guid containerId)
        {
            var result = await _mongoCollection.FindAsync(CreateFindByContainerIdFilter(containerId));
            var volumes = await result.ToListAsync();

            return volumes.ToArray();
        }

        public async Task<Volume> FindByIdAsync(Guid volumeId)
        {
            var result = await _mongoCollection.FindAsync(CreateFindByIdFilter(volumeId));

            return await result.FirstOrDefaultAsync();
        }

        private FilterDefinition<Volume> CreateFindByIdFilter(Guid id)
        {
            return Builders<Volume>.Filter.Where(c => c.Id == id);
        }

        public async Task DeleteAsync(Volume volume)
        {
            await _mongoCollection.DeleteOneAsync(CreateFindByIdFilter(volume.Id));
        }

        public async Task SaveAsync(Volume volume)
        {
            await _mongoCollection.ReplaceOneAsync(
                CreateFindByIdFilter(volume.Id),
                volume,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        private FilterDefinition<Volume> CreateFindByContainerIdFilter(Guid containerId)
        {
            return Builders<Volume>.Filter.Where(c => c.ContainerId == containerId);
        }
    }
}
