using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Triceratops.Api.Models
{
    public class Container
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string DockerId { get; set; }

        public ObjectId ServerId { get; set; }

        public string ImageName { get; set; }

        public string ImageVersion { get; set; }

        public ushort Port { get; set; }

        public string[] Arguments { get; set; }
    }
}
