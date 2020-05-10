using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoContainerRepo : IContainerRepo
    {
        private IDbService DbService { get; }

        private IMongoCollection<Container> MongoCollection { get; }

        public MongoContainerRepo(IDbService dbService, IMongoCollection<Container> mongoCollection)
        {
            DbService = dbService;
            MongoCollection = mongoCollection;
        }

        public async Task DeleteAsync(Container container)
        {            
            await MongoCollection.DeleteOneAsync(CreateFindByIdFilter(container.Id));
        }

        public async Task<Container> FindByIdAsync(ObjectId id)
        {
            var result = await MongoCollection.FindAsync(CreateFindByIdFilter(id));

            return await result.FirstOrDefaultAsync();
        }

        public async Task<Container[]> FindByServerIdAsync(ObjectId serverId)
        {
            var result = await MongoCollection.FindAsync(CreateFindByServerIdFilter(serverId));
            var containers = await result.ToListAsync();

            return containers.ToArray();
        }

        public async Task SaveAsync(Container container)
        {
            if (container.Id == default)
            {
                container.Id = ObjectId.GenerateNewId();
            }

            await MongoCollection.ReplaceOneAsync(
                CreateFindByIdFilter(container.Id),
                container,
                new ReplaceOptions { IsUpsert = true }
            );
        }

        private FilterDefinition<Container> CreateFindByIdFilter(ObjectId id)
        {
            return Builders<Container>.Filter.Where(c => c.Id == id);
        }

        private FilterDefinition<Container> CreateFindByServerIdFilter(ObjectId serverId)
        {
            return Builders<Container>.Filter.Where(c => c.ServerId == serverId);
        }
    }
}
