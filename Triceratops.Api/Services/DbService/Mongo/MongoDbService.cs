using MongoDB.Driver;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Interfaces;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoDbService : IDbService
    {
        private const string DatabaseName = "triceratops";

        public IContainerRepo Containers { get; }

        public IServerRepo Servers { get; }

        private readonly MongoClient mongoClient;

        private readonly IMongoDatabase mongoDatabase;

        public MongoDbService()
        {
            mongoClient = CreateMongoClient();
            mongoDatabase = mongoClient.GetDatabase(DatabaseName);

            Containers = new MongoContainerRepo(this, mongoDatabase.GetCollection<Container>("container"));
            Servers = new MongoServerRepo(this, mongoDatabase.GetCollection<Server>("server"));
        }

        private MongoClient CreateMongoClient()
        {
            return new MongoClient(new MongoUrl("mongodb://root:password@triceratops.mongo"));
        }
    }
}
