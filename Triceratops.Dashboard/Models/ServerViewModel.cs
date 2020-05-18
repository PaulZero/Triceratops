using System;
using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.ResponseModels;
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

        public ServerDirectory[] Volumes => _storage?.Volumes
            ?? throw new Exception($"Storage is not set - Check {nameof(ServerViewModel)}.{nameof(HasStorageDetails)} before accessing.");

        public bool HasLogs => Logs?.ContainerLogItems?.Any() ?? false;

        public ServerLogResponse Logs { get; }

        public bool HasStorageDetails => _storage != null;

        private readonly ServerDetailsResponse _server;

        private readonly ServerInstance _storage;

        public ServerViewModel(ServerDetailsResponse server, ServerInstance storage = null, ServerLogResponse logs = null)
        {
            _server = server;
            _storage = storage;
            Logs = logs;
        }
    }
}
