using System;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Dashboard.Models
{
    public class ServerViewModel
    {
        public Guid Id => _server.ServerId;

        public string Name => _server.Name;

        public string Slug => _server.Slug;

        public string ServerType => _server.ServerType.ToString();

        public bool AllRunning => _server.Containers.All(c => c.State == ServerContainerState.Running);

        public IEnumerable<VolumeDirectory> Volumes { get; }

        public bool HasLogs => Logs?.ContainerLogItems?.Any() ?? false;

        public ServerLogResponse Logs { get; }

        private readonly ServerDetailsResponse _server;

        public ServerViewModel(ServerDetailsResponse server, IEnumerable<VolumeDirectory> storage = null, ServerLogResponse logs = null)
        {
            _server = server;
            Volumes = storage ?? new VolumeDirectory[0];
            Logs = logs;
        }
    }
}
