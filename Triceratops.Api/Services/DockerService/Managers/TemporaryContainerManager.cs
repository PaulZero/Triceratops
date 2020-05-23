using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Managers.ClientManager;
using Triceratops.Api.Services.DockerService.Models;

namespace Triceratops.Api.Services.DockerService.Managers
{
    public class TemporaryContainerManager : IDisposable
    {
        private readonly ClientManager.DockerClientManager _clientManager;

        private readonly TimeSpan _maxContainerAge;

        private readonly List<TemporaryContainer> _containers = new List<TemporaryContainer>();

        private readonly object _lockingObject = new object();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Task _destructionTask;

        private readonly ILogger _logger;

        public TemporaryContainerManager(DockerClientManager clientManager, TimeSpan maxContainerAge, ILogger logger)
        {
            _clientManager = clientManager;
            _maxContainerAge = maxContainerAge;
            _logger = logger;

            _destructionTask = Task.Run(RunDestructionLoop);
        }

        public TContainer GetContainer<TContainer>(Guid temporaryContainerId, bool refreshIfFound)
            where TContainer : TemporaryContainer
        {
            lock (_lockingObject)
            {
                var container = _containers.FirstOrDefault(c => c.TemporaryContainerId == temporaryContainerId);

                if (container is TContainer matchedContainer)
                {
                    if (refreshIfFound)
                    {
                        matchedContainer.RefreshLastAccessed();
                    }

                    return matchedContainer;
                }

                return null;
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

        private async Task RunDestructionLoop()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    lock (_lockingObject)
                    {
                        _logger.LogInformation("Starting temporary container cleanup.");

                        var containersToBeDeleted = _containers.Where(c => DateTime.Now - c.LastAccessed > _maxContainerAge).ToArray();

                        if (containersToBeDeleted.Any())
                        {
                            _logger.LogInformation($"{containersToBeDeleted.Length} container(s) to be cleaned up.");

                            using var dockerClient = _clientManager.CreateDockerClient();

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
            var dockerClient = _clientManager.CreateDockerClient();

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
