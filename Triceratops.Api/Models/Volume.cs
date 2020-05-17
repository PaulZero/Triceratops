using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Models
{
    public class Volume
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ContainerId { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public string ContainerMountPoint { get; set; }
    }
}
