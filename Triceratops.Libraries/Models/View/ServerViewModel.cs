using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Enums;

namespace Triceratops.Libraries.Models.View
{
    public class ServerViewModel
    {
        public string ServerId { get; set; }

        public string Name { get; set; }

        public ServerType ServerType { get; set; }

        [JsonIgnore]
        public bool AllRunning => Containers.All(c => c.IsRunning);

        public List<ContainerViewModel> Containers { get; set; }
    }
}
