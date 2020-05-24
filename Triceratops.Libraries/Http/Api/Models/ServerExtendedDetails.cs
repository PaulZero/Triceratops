using System.Collections.Generic;
using System.Linq;
using Triceratops.Libraries.Enums;
using Triceratops.Libraries.Models;

namespace Triceratops.Libraries.Http.Api.Models
{
    public class ServerExtendedDetails : ServerBasicDetails
    {
        public IEnumerable<ContainerBasicDetails> Containers { get; set; }

        public ServerExtendedDetails() : base()
        {
        }

        public ServerExtendedDetails(Server server, IEnumerable<ContainerBasicDetails> containers)
            : base(server, containers.All(c => c.State == ServerContainerState.Running))
        {
            Containers = containers;
        }
    }
}
