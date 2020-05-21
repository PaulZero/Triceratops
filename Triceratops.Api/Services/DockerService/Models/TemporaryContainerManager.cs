using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DockerService.Models
{
    public class TemporaryContainerManager : IDisposable
    {
        private readonly Func<DockerClient> _createClientCallback;

        private readonly TimeSpan _maxContainerAge;

        private readonly List<TemporaryContainer> _containers = new List<TemporaryContainer>();

        private readonly object _lockingObject = new object();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Task _destructionTask;

        private readonly ILogger _logger;

        public TemporaryContainerManager(Func<DockerClient> createClientCallback, TimeSpan maxContainerAge, ILogger logger)
        {
            _createClientCallback = createClientCallback;
            _maxContainerAge = maxContainerAge;
            _logger = logger;

            _destructionTask = Task.Run(RunDestructionLoop);
        }

        public TemporaryContainer GetContainer(Guid temporaryContainerId, bool refreshIfFound)
        {
            lock (_lockingObject)
            {
                var container = _containers.FirstOrDefault(c => c.TemporaryContainerId == temporaryContainerId);

                if (refreshIfFound && container != null)
                {
                    container.RefreshLastAccessed();
                }

                return container;
            }
        }

        public void AddContainer(TemporaryContainer container)
        {
            lock (_lockingObject)
            {
                if (_containers.Any(c => c.DockerContainerId == container.DockerContainerId))
                {
                    return;
                }

                _containers.Add(container);
            }
        }

        public async Task CleanupHangingContainers()
        {

        }

        private async Task RunDestructionLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    lock (_lockingObject)
                    {
                        _logger.LogInformation("Starting temporary container cleanup.");

                        var containersToBeDeleted = _containers.Where(c => (DateTime.Now - c.LastAccessed) > _maxContainerAge).ToArray();

                        if (containersToBeDeleted.Any())
                        {
                            _logger.LogInformation($"{containersToBeDeleted.Length} container(s) to be cleaned up.");

                            using var dockerClient = _createClientCallback();

                            foreach (var container in containersToBeDeleted)
                            {
                                _logger.LogInformation($"Cleaning up container {container.DockerContainerId}");

                                container.Destroy(dockerClient);

                                _containers.Remove(container);

                                _logger.LogInformation($"Finished cleaning up container {container.DockerContainerId}");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("No temporary containers to be cleaned up.");
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError($"Error cleaning up temporary containers: {exception.Message}");
                }
                finally
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), _cancellationTokenSource.Token);
                }
            }
        }

        public void Dispose()
        {
            var dockerClient = _createClientCallback();

            foreach (var container in _containers)
            {
                container.Destroy(dockerClient);
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource?.Dispose();
            _destructionTask?.Dispose();
        }
    }
}
