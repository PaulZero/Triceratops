using System;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DbService.Interfaces;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.Models.ServerConfiguration;

namespace Triceratops.Api.Services.ServerService
{
    public class ServerValidator : IServerValidator
    {
        private readonly IDbService _dbService;

        private Server[] _existingServers;

        public ServerValidator(IDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task ValidateServerAsync(Server server)
        {
            _existingServers = await _dbService.Servers.FindAllAsync();

            EnsureRequiredValuesAreSet(server);
            EnsurePortsAreAllUnique(server);
            EnsureServerNameAndSlugAreUnique(server);
            EnsureContainerNamesAreAllUnique(server);
            EnsureVolumeNamesAreAllUnique(server);
        }

        private void EnsureRequiredValuesAreSet(Server server)
        {
            EnsureServerHasValidValues(server);

            foreach (var container in server.Containers)
            {
                EnsureContainerHasValidValues(server, container);

                foreach (var volume in container.Volumes)
                {
                    EnsureVolumeHasValidValues(container, volume);
                }
            }
        }

        private void EnsureServerHasValidValues(Server server)
        {
            if (server.Id == default)
            {
                throw new Exception("Servers must have a valid ID set.");
            }

            if (string.IsNullOrWhiteSpace(server.JsonConfiguration))
            {
                throw new Exception("Servers must have a JSON configuration set.");
            }

            if (string.IsNullOrWhiteSpace(server.ConfigurationTypeName))
            {
                throw new Exception("Servers must have a configuration type name set.");
            }

            if (server.ConfigurationType == default)
            {
                throw new Exception("Servers must have a configuration type set.");
            }

            try
            {
                var config = server.DeserialiseConfiguration();

                if (!(config is AbstractServerConfiguration))
                {
                    throw new Exception($"Server configuration must be instance of {nameof(AbstractServerConfiguration)}.");
                }
            }
            catch
            {
                throw new Exception("Server configuration deserialisation must work correctly.");
            }
        }

        private void EnsureContainerHasValidValues(Server server, Container container)
        {
            if (container.Id == default)
            {
                throw new Exception("Containers must have a valid ID set.");
            }

            if (container.ServerId != server.Id)
            {
                throw new Exception("Containers must have the correct server ID set.");
            }

            if (string.IsNullOrWhiteSpace(container.Name))
            {
                throw new Exception("Containers must have a name specified.");
            }

            if (string.IsNullOrWhiteSpace(container.ImageName))
            {
                throw new Exception("Containers must have an image name specified.");
            }

            if (string.IsNullOrWhiteSpace(container.ImageVersion))
            {
                throw new Exception($"Containers must have an image tag specified.");
            }
        }

        private void EnsureVolumeHasValidValues(Container container, Volume volume)
        {
            if (volume.Id == default)
            {
                throw new Exception("Volumes must have a valid ID set.");
            }

            if (volume.ContainerId != container.Id)
            {
                throw new Exception("Volumes must have the correct container ID set.");
            }

            if (string.IsNullOrWhiteSpace(volume.DisplayName))
            {
                throw new Exception("Volumes must have a display name set.");
            }

            if (string.IsNullOrWhiteSpace(volume.DockerName))
            {
                throw new Exception("Volumes must have a docker name set.");
            }

            if (string.IsNullOrWhiteSpace(volume.ContainerMountPoint))
            {
                throw new Exception("Volumes have a container mount point set.");
            }
        }

        private void EnsurePortsAreAllUnique(Server server)
        {
            var boundPorts = server.HostPorts;            

            if (boundPorts.Distinct().Count() != boundPorts.Count())
            {
                throw new Exception("Two containers in this server are attempting to bind to the same host port.");
            }
            var existingBoundPorts = _existingServers.SelectMany(s => s.HostPorts);
            var portsInUse = boundPorts.Intersect(existingBoundPorts);

            if (portsInUse.Any())
            {
                var error = portsInUse.Count() == 1
                    ? $"Port {portsInUse.First()} is already in use."
                    : $"Ports {string.Concat(portsInUse)} are already in use.";

                throw new Exception(error);
            }
        }

        private void EnsureServerNameAndSlugAreUnique(Server server)
        {
            if (_existingServers.Any(s => s.Name == server.Name))
            {
                throw new Exception($"A server already exists with the name {server.Name}.");
            }

            if (_existingServers.Any(s => s.Slug == server.Slug))
            {
                throw new Exception($"A server already exists with the slug {server.Slug}.");
            }
        }

        private void EnsureContainerNamesAreAllUnique(Server server)
        {
            var existingContainerNames = _existingServers.SelectMany(s => s.Containers).Select(c => c.Name);
            var newContainerNames = server.Containers.Select(c => c.Name);
            var containerNamesInUse = existingContainerNames.Intersect(newContainerNames);
            
            if (containerNamesInUse.Any())
            {
                var error = containerNamesInUse.Count() == 1
                    ? $"Container name {containerNamesInUse.First()} is already in use."
                    : $"{containerNamesInUse.Count()} container names are already in use.";

                throw new Exception(error);
            }
        }

        private void EnsureVolumeNamesAreAllUnique(Server server)
        {
            var existingVolumes = _existingServers.SelectMany(s => s.Containers).SelectMany(c => c.Volumes);
            var existingDockerNames = existingVolumes.Select(v => v.DockerName);
            var existingDisplayNames = existingVolumes.SelectMany(v => v.DisplayName);

            var newVolumes = server.Containers.SelectMany(c => c.Volumes);
            var newDockerNames = newVolumes.Select(v => v.DockerName);
            var newDisplayNames = newVolumes.SelectMany(v => v.DisplayName);

            var dockerNamesInUse = existingDockerNames.Intersect(newDockerNames);

            if (dockerNamesInUse.Any())
            {
                var error = dockerNamesInUse.Count() == 1
                    ? $"Volume Docker name {dockerNamesInUse.First()} is already in use."
                    : $"{dockerNamesInUse.Count()} volume Docker names are already in use.";

                throw new Exception(error);
            }

            var displayNamesInUse = existingDisplayNames.Intersect(newDisplayNames);

            if (displayNamesInUse.Any())
            {
                var error = displayNamesInUse.Count() == 1
                    ? $"Volume display name {displayNamesInUse.First()} is already in use."
                    : $"{displayNamesInUse.Count()} volume display names are already in use.";

                throw new Exception(error);
            }
        }
    }
}
