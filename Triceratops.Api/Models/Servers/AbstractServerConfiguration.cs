using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Models.Servers
{
    public abstract class AbstractServerConfiguration
    {
        public abstract bool IsValid();
    }
}
