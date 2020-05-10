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
        private IDbService DbService { get; }

        private IMongoCollection<Server> MongoCollection { get; }

        public MongoServerRepo(IDbService dbService, IMongoCollection<Server> mongoCollection)
        {
            DbService = dbService;
            MongoCollection = mongoCollection;
        }

        public async Task DeleteAsync(Server server)
        {
            await MongoCollection.DeleteOneAsync(CreateFindByIdFilter(server.Id));
        }

        public async Task<Server[]> FindAllAsync()
        {
            var result = await MongoCollection.FindAsync(Builders<Server>.Filter.Empty);
            var items = await result.ToListAsync();

            foreach (var server in items)
            {
                await PopulateContainers(server);
            }

            return items.ToArray();
        }

        public async Task<Server> FindByIdAsync(Guid id)
        {
            var result = await MongoCollection.FindAsync(CreateFindByIdFilter(id));

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

            await MongoCollection.ReplaceOneAsync(
                CreateFindByIdFilter(server.Id),
                server,
                new ReplaceOptions { IsUpsert = true }
            );

            foreach (var container in server.Containers)
            {
                await DbService.Containers.SaveAsync(container);
            }
        }

        private async Task PopulateContainers(Server server)
        {
            var collections = await DbService.Containers.FindByServerIdAsync(server.Id);

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
