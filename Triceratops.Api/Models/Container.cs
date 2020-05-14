using System;

namespace Triceratops.Api.Models
{
    public class Container
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string DockerId { get; set; }

        public Guid ServerId { get; set; }

        public string ImageName { get; set; }

        public string ImageVersion { get; set; }

        public ServerPorts[] ServerPorts { get; set; }

        public string[] Arguments { get; set; }
    }
}
