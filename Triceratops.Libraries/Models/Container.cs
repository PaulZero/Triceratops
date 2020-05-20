using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Triceratops.Libraries.Models
{
    public class Container
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string DockerId { get; set; }

        public Guid ServerId { get; set; }

        public string ImageName { get; set; }

        public string ImageVersion { get; set; }

        public List<ServerPorts> ServerPorts { get; set; } = new List<ServerPorts>();

        public List<Volume> Volumes { get; set; } = new List<Volume>();

        public List<string> Arguments { get; set; } = new List<string>();
    }
}
