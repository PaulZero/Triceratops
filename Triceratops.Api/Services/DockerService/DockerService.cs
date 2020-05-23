using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Builders;
using Triceratops.Api.Services.DockerService.Helpers;
using Triceratops.Api.Services.DockerService.Managers;
using Triceratops.Api.Services.DockerService.Managers.ClientManager;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Api.Services.DockerService.ServiceResponses;
using Triceratops.Libraries.Http.Core;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DockerService
{
    public class DockerService : IDockerService
    {
        private readonly DockerClientManager _clientManager;
        private readonly TemporaryContainerManager _temporaryContainerManager;
        private readonly ILogger _logger;
        private bool _hasPrepareBeenCalled = false;

        public DockerService(Uri dockerDaemonUri, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<IDockerService>();

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

        //public Task<string[]> GetContainerLogAsync(string containerId, uint rows = 300)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ContainerStatusResponse> GetContainerStatusAsync(Container container)
        {
            var response = await _clientManager.InspectContainerAsync(container.DockerId);

            if (response.Success)
            {
                return new ContainerStatusResponse(response, response.DockerResponse);
            }

            return new ContainerStatusResponse(response);
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
            var response = await _clientManager.StartContainerAsync(containerId);

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
