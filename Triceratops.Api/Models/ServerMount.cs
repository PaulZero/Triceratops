using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Models
{
    public class ServerMount
    {
        public string HostDirectory { get; set; }

        public string ContainerDirectory { get; set; }
    }
}
