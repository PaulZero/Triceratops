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

        private readonly MongoClient _mongoClient;

        private readonly IMongoDatabase _mongoDatabase;

        private readonly string _mongoUsername;

        private readonly string _mongoPassword;

        public MongoDbService(string mongoUsername, string mongoPassword)
        {
            _mongoUsername = mongoUsername;
            _mongoPassword = mongoPassword;

            _mongoClient = CreateMongoClient();
            _mongoDatabase = _mongoClient.GetDatabase(DatabaseName);

            Containers = new MongoContainerRepo(this, _mongoDatabase.GetCollection<Container>("container"));
            Servers = new MongoServerRepo(this, _mongoDatabase.GetCollection<Server>("server"));
            Volumes = new MongoVolumeRepo(_mongoDatabase.GetCollection<Volume>("volume"));
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

            return new MongoClient(new MongoUrl($"mongodb://{_mongoUsername}:{_mongoPassword}@Triceratops.Mongo"));
        }

        public ITransaction StartTransaction()
        {
            return new MongoTransaction(_mongoClient.StartSession());
        }
    }
}
