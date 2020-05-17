using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.Api.Response;
using Triceratops.Libraries.Models.ServerConfiguration;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Models
{
    public class ServerViewModel
    {
        public Guid Id => _server.Id;

        public string Name => _server.Name;

        public string Slug => _server.Slug;

        public string ServerType => _server.ServerType.ToString();

        public bool AllRunning => _server.Containers.All(c => c.State == ServerContainerState.Running);

        public ServerDirectory[] Volumes => _storage?.Directories 
            ?? throw new Exception($"Storage is not set - Check {nameof(ServerViewModel)}.{nameof(HasStorageDetails)} before accessing.");

        public bool HasStorageDetails => _storage != null;

        private readonly ServerResponse _server;

        private readonly ServerStorage _storage;

        public ServerViewModel(ServerResponse server, ServerStorage storage = null)
        {
            _server = server;
            _storage = storage;
        }
    }
}
