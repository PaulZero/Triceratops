using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.DockerService.Builders;
using Triceratops.DockerService.Helpers;
using Triceratops.DockerService.Managers;
using Triceratops.DockerService.Managers.ClientManager;
using Triceratops.DockerService.Models;
using Triceratops.DockerService.ServiceResponses;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models;

namespace Triceratops.DockerService
{
    public class TriceratopsDockerClient : ITriceratopsDockerClient
    {
        private readonly DockerClientManager _clientManager;
        private readonly TemporaryContainerManager _temporaryContainerManager;
        private readonly ILogger _logger;
        private bool _hasPrepareBeenCalled = false;

        public TriceratopsDockerClient(Uri dockerDaemonUri, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ITriceratopsDockerClient>();

            _clientManager = new DockerClientManager(dockerDaemonUri, loggerFactory);
            _temporaryContainerManager = new TemporaryContainerManager(
                _clientManager,
                TimeSpan.FromMinutes(2),
                loggerFactory.CreateLogger<TemporaryContainerManager>()
            );

            PrepareAsync().Wait();
        }

        public async Task<DockerOperationResponse> CreateContainerAsync(Container container)
        {
            var builder = CreateContainerParametersBuilder.CreateFromContainer(container);
            var response = await _clientManager.CreateContainerAsync(builder);

            if (response.Success)
            {
                container.DockerId = response.ContainerId;
            }

            return new DockerOperationResponse(
                response.Success,
                response.Errors,
                response.Warnings
            );
        }

        public async Task<DockerOperationResponse> DeleteContainerAsync(Container container)
        {
            var response = await _clientManager.DeleteContainerById(container.DockerId);

            return new DockerOperationResponse(
                response.Success,
                response.Errors,
                response.Warnings
            );
        }

        public async Task<ContainerStatusResponse> GetContainerStatusAsync(Container container)
        {
            var response = await _clientManager.InspectContainerAsync(container.DockerId);

            if (response.Success)
            {
                return new ContainerStatusResponse(response, response.DockerResponse);
            }

            return new ContainerStatusResponse(response);
        }

        public async Task<DockerStreamResponse> GetContainerLogStreamAsync(string containerId)
        {
            var stream = await _clientManager.GetContainerLogStreamAsync(containerId, true);

            if (stream == null)
            {
                return new DockerStreamResponse();
            }

            return new DockerStreamResponse(stream);
        }

        public async Task<TemporaryContainerResponse<TemporaryStorageContainer>> GetStorageContainerAsync(Server server)
        {
            var existingContainer = _temporaryContainerManager.GetContainer<TemporaryStorageContainer>(server.Id, true);

            if (existingContainer != null)
            {
                return new TemporaryContainerResponse<TemporaryStorageContainer>(existingContainer);
            }

            var parameterBuilder = CreateContainerParametersBuilder.CreateStorageServer(server);

            var createResponse = await _clientManager.CreateContainerAsync(parameterBuilder);

            if (!createResponse.Success)
            {
                return new TemporaryContainerResponse<TemporaryStorageContainer>(createResponse);
            }

            var startResponse = await _clientManager.StartContainerAsync(createResponse.ContainerId);

            if (!startResponse.Success)
            {
                return new TemporaryContainerResponse<TemporaryStorageContainer>(startResponse);
            }


            var httpClient = CoreHttpClient.Create(_logger, $"http://{parameterBuilder.CreateParameters().Name}");
            var container = new TemporaryStorageContainer(httpClient, server.Id, createResponse.ContainerId);

            _temporaryContainerManager.AddContainer(container);

            await DockerRetryHelper.WaitForStorageContainerAsync(container, _logger);

            return new TemporaryContainerResponse<TemporaryStorageContainer>(container);
        }

        public async Task<DockerOperationResponse> StartContainerAsync(string containerId, bool waitForStart = default)
        {
            var response = await _clientManager.StartContainerAsync(containerId);

            if (!response.Success)
            {
                return new DockerOperationResponse(response);
            }

            if (waitForStart)
            {
                if (!await DockerRetryHelper.WaitForContainerToStartAsync(containerId, _clientManager, _logger))
                {
                    response.LogWarning($"Timed out waiting for {containerId} to start, it may have stalled.");
                }

            }

            return new DockerOperationResponse(response);
        }

        public async Task<DockerOperationResponse> StopContainerAsync(string containerId, bool waitForStop = false)
        {
            var response = await _clientManager.StopContainerAsync(containerId);

            if (!response.Success)
            {
                return new DockerOperationResponse(response);
            }

            if (waitForStop)
            {
                if (!await DockerRetryHelper.WaitForContainerToStopAsync(containerId, _clientManager, _logger))
                {
                    response.LogWarning($"Timed out waiting for {containerId} to start, it may have stalled.");
                }
            }

            return new DockerOperationResponse(response);
        }

        private async Task PrepareAsync()
        {
            if (_hasPrepareBeenCalled)
            {
                return;
            }

            // The app is starting up, so delete any hanging temporary containers because
            // any handles we had to them are long gone.
            await _clientManager.DeleteAllTemporaryContainersAsync();
        }
    }
}
