using MongoDB.Bson.Serialization;
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
            BsonClassMap.RegisterClassMap<Server>(s =>
            {
                s.AutoMap();
                s.MapIdField(s => s.Id);
                s.UnmapProperty(s => s.Containers);
                s.UnmapProperty(s => s.ConfigurationType);
                s.UnmapProperty(s => s.Ports);
            });

            BsonClassMap.RegisterClassMap<Container>(s =>
            {
                s.AutoMap();
                s.MapIdField(s => s.Id);
            });

            return new MongoClient(new MongoUrl("mongodb://root:password@triceratops.mongo"));
        }
    }
}
