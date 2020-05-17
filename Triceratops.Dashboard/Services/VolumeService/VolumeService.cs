using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Dashboard.Services.VolumeService.Interfaces;
using Triceratops.Libraries.Http;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Services.VolumeService
{
    public class VolumeService : AbstractHttpClient, IVolumeService
    {
        public VolumeService(ILogger logger) : base("http://triceratops.volumemanager", logger)
        {
        }

        public Task<ServerStorage> GetServerAsync(string serverName)
            => GetAsync<ServerStorage>($"/servers/{serverName}");

        public Task<string[]> GetServerNamesAsync()
            => GetAsync<string[]>("/servers");
    }
}
