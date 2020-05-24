using System;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models;

namespace Triceratops.Libraries.Http.Api.Models
{
    public class ServerBasicDetails
    {
        public Guid ServerId { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public ServerType ServerType { get; set; }

        public bool IsRunning { get; set; }

        public ServerBasicDetails()
        {
        }

        public ServerBasicDetails(Server server, bool isRunning)
        {
            var config = server.DeserialiseConfiguration();

            ServerId = server.Id;
            Slug = server.Slug;
            ServerType = config.ServerType;
            Name = server.Name;
            IsRunning = isRunning;
        }
    }
}
