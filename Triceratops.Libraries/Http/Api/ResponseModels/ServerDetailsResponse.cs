using System;
using System.Collections.Generic;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerDetailsResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public ServerType ServerType { get; set; }

        public List<ContainerResponse> Containers { get; set; } = new List<ContainerResponse>();

        public ServerDetailsResponse()
        {
        }

        public ServerDetailsResponse(Server server)
        {
            var configuration = server.DeserialiseConfiguration() as AbstractServerConfiguration;

            Id = server.Id;
            Name = server.Name;
            Slug = server.Slug;
            ServerType = configuration.ServerType;
        }
    }
}
