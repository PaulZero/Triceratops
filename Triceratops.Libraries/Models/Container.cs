using System;

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

        public ServerPorts[] ServerPorts { get; set; } = new ServerPorts[0];

        public Volume[] Volumes { get; set; } = new Volume[0];

        public string[] Arguments { get; set; }
    }
}
