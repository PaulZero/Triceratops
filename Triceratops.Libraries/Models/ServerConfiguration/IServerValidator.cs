using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Triceratops.Libraries.Models.ServerConfiguration
{
    public interface IServerValidator
    {
        Task ValidateServerAsync(Server server);
    }
}
