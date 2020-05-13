using System;

namespace Triceratops.Libraries.Models.View
{
    public class ContainerViewModel
    {
        public string Name { get; set; }

        public string DockerId { get; set; }

        public string Status { get; set; }

        public bool IsRunning { get; set; }

        public DateTime Created { get; set; }
    }
}
