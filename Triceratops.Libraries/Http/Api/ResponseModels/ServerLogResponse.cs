using System;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerLogResponse : IServerApiResponse
    {
        public Guid ServerId { get; set; }

        public bool Success { get; set; } = true;

        public string Error { get; set; }

        public string ServerName { get; set; }

        public ContainerLogItem[] ContainerLogItems { get; set; }       

        public class ContainerLogItem
        {
            public Guid ContainerId { get; set; }

            public string ContainerName { get; set; }

            public string[] LogRows { get; set; }
        }
    }
}
