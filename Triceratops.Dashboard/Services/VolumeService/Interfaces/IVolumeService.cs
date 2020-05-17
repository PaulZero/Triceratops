using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Services.VolumeService.Interfaces
{
    public interface IVolumeService
    {
        Task<string[]> GetServerNamesAsync();

        Task<ServerStorage> GetServerAsync(string serverName);
    }
}
