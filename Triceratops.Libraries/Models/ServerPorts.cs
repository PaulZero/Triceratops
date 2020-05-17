using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Libraries.Models
{
    public class ServerPorts
    {
        public ushort HostPort { get; set; }

        public ushort ContainerPort { get; set; }
    }
}
