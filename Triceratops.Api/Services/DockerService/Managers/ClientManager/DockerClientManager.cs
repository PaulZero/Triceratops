using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DockerService.Builders;
using Triceratops.Api.Services.DockerService.Enums;
using Triceratops.Api.Services.DockerService.Helpers;
using Triceratops.Api.Services.DockerService.Managers.ClientManager.Responses;
using Triceratops.Api.Services.DockerService.Models;
using Triceratops.Api.Services.DockerService.Structs;

namespace Triceratops.Api.Services.DockerService.Managers.ClientManager
{
    public class DockerClientManager
    {
        private readonly Uri _dockerDaemonUri;
        private readonly ImageSourceManager _imageSourceManager;
        private readonly ILogger _logger;

        public DockerClientManager(Uri dockerDaemonUri, ILoggerFactory loggerFactory)
        {
            _dockerDaemonUri = dockerDaemonUri;
            _imageSourceManager = new ImageSourceManager(loggerFactory.CreateLogger<ImageSourceManager>());
            _logger = loggerFactory.CreateLogger<DockerClientManager>();
        }

        public DockerClient CreateDockerClient()
        {
            using var loggerScope = _logger.BeginScope("Creating Docker client");

            try
            {
                return new DockerClientConfiguration(_dockerDaemonUri)
                    .CreateClient();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not create Docker client");

                throw;
            }
        }

        public async Task<ContainerIdResponse> CreateContainerAsync(CreateContainerParametersBuilder builder)
        {
            using var loggerScope = _logger.BeginScope("Creating new container");
            var response = new ContainerIdResponse(_logger);

            builder.LogParameters(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                await PrepareImageAsync(builder.ImageId, dockerClient);

                var dockerResponse = await dockerClient.Containers.CreateContainerAsync(builder.CreateParameters());

                if (dockerResponse.Warnings.Any())
                {
                    using (_logger.BeginScope("Container warnings were generated"))
                    {
                        foreach (var warning in dockerResponse.Warnings)
                        {
                            response.LogWarning($"Docker warning: {warning}");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(dockerResponse.ID))
                {
                    throw new Exception("No container ID received from created container.");
                }

                response.WithContainerId(dockerResponse.ID);
            }
            catch (Exception exception)
            {
                response.LogException(exception, "Unable to create new container");

                response.HasFailed();
            }

            return response;
        }

        public async Task<LoggedResponse> DeleteContainerById(string containerId)
        {
            using var loggerScope = _logger.BeginScope($"Deleting container {containerId}");
            var response = new LoggedResponse(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                if (!await EnsureContainerWasCreatedByTriceratops(containerId, response, dockerClient))
                {
                    return response.HasFailed();
                }

                await dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters
                {
                    Force = true,
                    RemoveVolumes = false
                });

                _logger.LogInformation("Container deleted");
            }
            catch (Exception exception)
            {
                response
                    .LogException(exception, $"Unable to delete container")
                    .HasFailed();
            }

            return response;
        }

        public async Task<BatchOperationResponse> DeleteAllInternalContainersAsync()
        {
            using var loggerScope = _logger.BeginScope("Deleting all internal containers");

            var filters = DockerFilterCollection.Build(
                (DockerFilterField.Label, DockerLabelHelper.CreatedByTriceratopsLabel)
            );

            return await DeleteContainersWithFiltersAsync(filters, true, true, false);
        }

        public async Task<BatchOperationResponse> DeleteAllTemporaryContainersAsync()
        {
            using var loggerScope = _logger.BeginScope("Deleting all temporary containers");

            var filters = DockerFilterCollection.Build(
                (DockerFilterField.Label, DockerLabelHelper.TemporaryContainerLabel)
            );

            return await DeleteContainersWithFiltersAsync(filters, true, true, false);
        }

        public async Task<RawDockerResponse<ContainerInspectResponse>> InspectContainerAsync(string containerId)
        {
            using var loggerScope = _logger.BeginScope($"Inspecting container {containerId}");
            var response = new RawDockerResponse<ContainerInspectResponse>(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                response.WithResponse(await dockerClient.Containers.InspectContainerAsync(containerId));

                _logger.LogInformation("Container inspected successfully");
            }
            catch (Exception exception)
            {
                response.LogException(exception, "Unable to inspect container");

                response.HasFailed();
            }

            return response;
        }

        public async Task<LoggedResponse> RestartContainerAsync(string containerId)
        {
            using var loggerScope = _logger.BeginScope($"Restarting container {containerId}");
            var response = new LoggedResponse(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                if (!await EnsureContainerWasCreatedByTriceratops(containerId, response, dockerClient))
                {
                    return response.HasFailed();
                }

                await dockerClient.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters());

                _logger.LogInformation("Container has been restarted");

                return response.IsSuccessful();
            }
            catch (Exception exception)
            {
                return response
                    .LogException(exception, "Unable to restart container")
                    .HasFailed();
            }
        }

        public async Task<LoggedResponse> StartContainerAsync(string containerId)
        {
            using var loggerScope = _logger.BeginScope($"Starting container {containerId}");
            var response = new LoggedResponse(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                if (!await EnsureContainerWasCreatedByTriceratops(containerId, response, dockerClient))
                {
                    return response.HasFailed();
                }

                await dockerClient.Containers.StartContainerAsync(containerId, new ContainerStartParameters());

                _logger.LogInformation("Container has been started");

                return response.IsSuccessful();
            }
            catch (Exception exception)
            {
                return response
                    .LogException(exception, "Unable to start container")
                    .HasFailed();
            }
        }

        public async Task<LoggedResponse> StopContainerAsync(string containerId)
        {
            using var loggerScope = _logger.BeginScope($"Stopping container {containerId}");
            var response = new LoggedResponse(_logger);

            try
            {
                using var dockerClient = CreateDockerClient();

                if (!await EnsureContainerWasCreatedByTriceratops(containerId, response, dockerClient))
                {
                    return response.HasFailed();
                }

                await dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters());

                _logger.LogInformation("Container has been stopped");

                return response.IsSuccessful();
            }
            catch (Exception exception)
            {
                return response
                    .LogException(exception, "Unable to stop container")
                    .HasFailed();
            }
        }

        private async Task<bool> BuildImageSourceAsync(
            DockerImageId imageId,
            DirectoryInfo sourceDirectory,
            DockerClient dockerClient
        )
        {
            using var loggerScope = _logger.BeginScope($"Building docker image from {sourceDirectory.FullName}"); 
            
            var response = new LoggedResponse(_logger);

            try
            {
                if (await CheckImageExistsAsync(imageId, dockerClient))
                {
                    return true;
                }

                _logger.LogInformation("Creating tar stream from directory");

                var builder = new TarBuilder();
                var sourceStream = await builder.BuildFromDirectory(sourceDirectory);

                _logger.LogInformation("Creating Docker image from tar stream");

                using var dockerResponseStream = await dockerClient.Images.BuildImageFromDockerfileAsync(sourceStream, new ImageBuildParameters
                {
                    Tags = new List<string>
                    {
                        imageId.FullName
                    },
                    Labels = DockerLabelHelper.CreateBaseLabels()
                });

                if (!await DockerBuildHelper.ValidateBuildAsync(dockerResponseStream, _logger))
                {
                    throw new Exception($"Docker build returned an error, lol soz (TODO: maybe log the error, Paul?)");
                }

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to build Docker image");
            }

            return false;
        }

        private async Task<bool> CheckImageExistsAsync(DockerImageId imageId, DockerClient dockerClient)
        {
            using var loggerScope = _logger.BeginScope($"Checking Docker image {imageId.FullName} exists locally");

            try
            {
                await dockerClient.Images.InspectImageAsync(imageId.FullName);

                _logger.LogInformation("The image exists already");

                return true;
            }
            catch
            {
                _logger.LogInformation("The image does not exist");

                return false;
            }
        }

        private async Task<BatchOperationResponse> DeleteContainersWithFiltersAsync(
            DockerFilterCollection filters,
            bool ignoreEmptyResult,
            bool force,
            bool removeVolumes
        )
        {
            var response = new BatchOperationResponse(_logger);

            filters.RequiresLabel(DockerLabelHelper.CreatedByTriceratopsLabel);

            using (_logger.BeginScope("Using filters to delete containers"))
            {
                filters.LogContents(_logger);
            }

            try
            {
                using var dockerClient = CreateDockerClient();

                IList<ContainerListResponse> existingContainers = null;

                using (_logger.BeginScope("Finding existing containers to delete"))
                {
                    existingContainers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
                    {
                        Filters = filters.ToDictionary()
                    });

                    if (!existingContainers.Any())
                    {
                        if (ignoreEmptyResult)
                        {
                            _logger.LogInformation("The filter parameters supplied matched no containers, " +
                                "but empty result is being ignored.");

                            return response.IsSuccessful();
                        }

                        return response
                            .LogWarning("The filter parameters supplied matched no containers")
                            .HasFailed();
                    }

                    _logger.LogInformation($"Found {existingContainers.Count} containers to delete");
                }

                foreach (var container in existingContainers)
                {
                    using (_logger.BeginScope($"Deleting container {container.ID}"))
                    {
                        try
                        {
                            await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                            {
                                Force = force,
                                RemoveVolumes = removeVolumes
                            });

                            response.AddSuccessfulContainerId(container.ID);

                            _logger.LogInformation("Container deleted");
                        }
                        catch (Exception exception)
                        {
                            response
                                .LogException(exception, $"Unable to delete container {container.ID}")
                                .AddFailedContainerId(container.ID);
                        }
                    }
                }

                if (response.SuccessCount == response.TotalOperations)
                {
                    return response.IsSuccessful();
                }

                return response
                    .LogError($"Expected to delete {response.TotalOperations}, only deleted {response.SuccessCount}")
                    .HasFailed();
            }
            catch (Exception exception)
            {
                response
                    .LogException(exception, "Unable to delete containers")
                    .HasFailed();
            }

            return response;
        }

        private async Task<bool> DownloadImageAsync(DockerImageId imageId, DockerClient dockerClient)
        {
            using var loggerScope = _logger.BeginScope($"Download docker image {imageId.FullName}");

            try
            {
                if (await CheckImageExistsAsync(imageId, dockerClient))
                {
                    return true;
                }

                var parameters = new ImagesCreateParameters
                {
                    FromImage = imageId.Name,
                    Tag = imageId.Tag
                };

                await dockerClient.Images.CreateImageAsync(parameters, null, null);

                _logger.LogInformation("Image downloaded");

                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to download image");

                return false;
            }
        }

        private async Task<bool> EnsureContainerWasCreatedByTriceratops(
            string containerId,
            LoggedResponse response,
            DockerClient dockerClient
        )
        {
            using var loggerScope = _logger.BeginScope("Checking container was created by Triceratops");

            try
            {
                var inspectResponse = await dockerClient.Containers.InspectContainerAsync(containerId);

                if (!inspectResponse.Config.Labels.ContainsKey(DockerLabelHelper.CreatedByTriceratopsLabel))
                {
                    response.LogError($"The container was not created by Triceratops!");

                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                response.LogException(exception, "Unable to check container was created by Triceratops");
            }

            return false;
        }

        private async Task<bool> PrepareImageAsync(DockerImageId imageId, DockerClient dockerClient)
        {
            using var loggerScope = _logger.BeginScope($"Preparing image {imageId.FullName}");

            if (_imageSourceManager.IsInternalImage(imageId))
            {
                var source = _imageSourceManager.Get(imageId);

                return await BuildImageSourceAsync(imageId, source.Directory, dockerClient);
            }

            return await DownloadImageAsync(imageId, dockerClient);
        }
    }
}
