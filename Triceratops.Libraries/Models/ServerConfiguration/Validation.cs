using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Triceratops.Libraries.Models.ServerConfiguration
{
    public static class Validation
    {
        public static bool ValidatePortsInRange(params ushort[] ports)
        {
            if (ports.Any(p => p < 1))
            {
                return false;
            }

            return true;
        }

        public static bool ValidatePortsDistinct(params ushort[] ports)
        {
            return ports.Distinct().Count() == ports.Length;
        }
    }
}
