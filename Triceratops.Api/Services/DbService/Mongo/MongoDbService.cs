using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DbService.Mongo
{
    public class MongoDbService : IDbService
    {
        private const string DatabaseName = "triceratops";

        public IContainerRepo Containers { get; }

        public IServerRepo Servers { get; }

        public IVolumeRepo Volumes { get; }

        private readonly MongoClient mongoClient;

        private readonly IMongoDatabase mongoDatabase;

        public MongoDbService()
        {
            mongoClient = CreateMongoClient();
            mongoDatabase = mongoClient.GetDatabase(DatabaseName);

            Containers = new MongoContainerRepo(this, mongoDatabase.GetCollection<Container>("container"));
            Servers = new MongoServerRepo(this, mongoDatabase.GetCollection<Server>("server"));
            Volumes = new MongoVolumeRepo(mongoDatabase.GetCollection<Volume>("volume"));
        }

        private MongoClient CreateMongoClient()
        {
            BsonClassMap.RegisterClassMap<Server>(s =>
            {
                s.AutoMap();
                s.MapIdField(s => s.Id);
                s.UnmapProperty(s => s.Containers);
                s.UnmapProperty(s => s.ConfigurationType);
                s.UnmapProperty(s => s.HostPorts);
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
