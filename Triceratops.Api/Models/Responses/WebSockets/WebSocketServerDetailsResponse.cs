using System;
using System.Linq;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Models.Responses.WebSockets
{
    public class WebSocketServerDetailsResponse : WebSocketApiResponse
    {
        public Guid ServerId { get; }

        public string ServerSlug { get; }

        public bool IsRunning { get; }

        public WebSocketServerDetailsResponse(Server server)
        {
            ServerId = server.Id;
            ServerSlug = server.Slug;
            IsRunning = true; // TODO: This is bollocks
        }
    }
}
