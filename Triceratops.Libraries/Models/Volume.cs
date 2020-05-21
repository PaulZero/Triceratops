using System;

namespace Triceratops.Libraries.Models
{
    public class Volume
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the display name for this volume. This is how the volume will be displayed
        /// within the dashboard.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the Docker name for this volume. This is the horridly namespaced name that
        /// is used within Docker but isn't suitable for human consumption.
        /// </summary>
        public string DockerName { get; set; }

        /// <summary>
        /// Gets or sets the mount point within the container. This will be the path the image is
        /// expecting some magical directory.
        /// </summary>
        public string ContainerMountPoint { get; set; }
    }
}
