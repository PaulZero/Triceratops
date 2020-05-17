using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoServerRepo : IServerRepo
    {
        private readonly IDbService _dbService;

        private readonly IMongoCollection<Server> _mongoCollection;

        public MongoServerRepo(IDbService dbService, IMongoCollection<Server> mongoCollection)
        {
            _dbService = dbService;
            _mongoCollection = mongoCollection;
        }

        public async Task DeleteAsync(Server server)
        {
            await _mongoCollection.DeleteOneAsync(CreateFindByIdFilter(server.Id));
        }

        public async Task<Server[]> FindAllAsync()
        {
            var result = await _mongoCollection.FindAsync(Builders<Server>.Filter.Empty);
            var items = await result.ToListAsync();

            foreach (var server in items)
            {
                await PopulateContainers(server);
            }

            return items.ToArray();
        }

        public async Task<Server> FindByIdAsync(Guid id)
        {
            var result = await _mongoCollection.FindAsync(CreateFindByIdFilter(id));

            return await result.FirstOrDefaultAsync();
        }

        public async Task SaveAsync(Server server)
        {
            foreach (var container in server.Containers)
            {
                if (container.ServerId == default)
                {
                    container.ServerId = server.Id;
                }                
            }

            await _mongoCollection.ReplaceOneAsync(
                CreateFindByIdFilter(server.Id),
                server,
                new ReplaceOptions { IsUpsert = true }
            );

            foreach (var container in server.Containers)
            {
                await _dbService.Containers.SaveAsync(container);
            }
        }

        private async Task PopulateContainers(Server server)
        {
            var collections = await _dbService.Containers.FindByServerIdAsync(server.Id);

            if (collections.Any())
            {
                server.Containers.AddRange(collections);
            }
        }

        private FilterDefinition<Server> CreateFindByIdFilter(Guid id)
        {
            return Builders<Server>.Filter.Where(s => s.Id == id);
        }
    }
}
