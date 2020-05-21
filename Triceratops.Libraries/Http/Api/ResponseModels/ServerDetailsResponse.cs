using System;
using System.Collections.Generic;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.Interfaces;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerDetailsResponse : IServerApiResponse
    {
        public Guid ServerId { get; set; }

        public bool Success { get; set; } = true;

        public string Error { get; set; }

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

            ServerId = server.Id;
            Name = server.Name;
            Slug = server.Slug;
            ServerType = configuration.ServerType;
        }
    }
}
