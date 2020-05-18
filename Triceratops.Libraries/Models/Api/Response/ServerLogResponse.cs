using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Models.Api.Response
{
    public class ServerLogResponse
    {
        public Guid ServerId { get; set; }

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
