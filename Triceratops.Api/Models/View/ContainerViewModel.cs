using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Models;

namespace Triceratops.Api.Models.View
{
    public class ContainerViewModel
    {
        public string Name => _container.Name;

        public string DockerId => _container.DockerId;

        public string Status => _details.Status;

        public bool IsRunning => _details.IsRunning;

        public DateTime Created => _details.Created;

        private Container _container;
        private ContainerDetails _details;

        public ContainerViewModel(Container container, ContainerDetails details)
        {
            _container = container;
            _details = details;
        }

    }
}
