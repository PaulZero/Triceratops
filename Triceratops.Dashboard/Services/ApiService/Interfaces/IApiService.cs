using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Dashboard.Services.ApiService.Interfaces
{
    public interface IApiService
    {
        IServerApi Servers { get; }
    }
}
