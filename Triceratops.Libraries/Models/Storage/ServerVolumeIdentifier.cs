using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Models.Storage
{
    public class ServerVolumeIdentifier
    {
        public string ContainerName { get; }

        public string VolumeName { get; }

        public string VolumeDisplayName { get; }

        public ServerVolumeIdentifier(Container container, Volume volume)
        {
            ContainerName = container.DisplayName;
            VolumeName = volume.DockerName;
            VolumeDisplayName = volume.DisplayName;
        }

        public string CreateMountDestination(string rootPath)
        {
            var destination = new StringBuilder();

            if (!rootPath.StartsWith('/'))
            {
                destination.Append('/');
            }

            destination.Append(rootPath);

            if (!rootPath.EndsWith('/') && !ContainerName.StartsWith('/'))
            {
                destination.Append('/');
            }

            destination.Append(ContainerName);

            if (!ContainerName.EndsWith('/') && !VolumeDisplayName.EndsWith('/'))
            {
                destination.Append('/');
            }

            destination.Append(VolumeDisplayName);

            return destination.ToString();
        }

        public static IEnumerable<ServerVolumeIdentifier> CreateForServer(Server server)
        {
            var volumes = new List<ServerVolumeIdentifier>();

            foreach (var container in server.Containers)
            {
                foreach (var volume in container.Volumes)
                {
                    volumes.Add(new ServerVolumeIdentifier(container, volume));
                }
            }

            return volumes;
        }
    }
}
