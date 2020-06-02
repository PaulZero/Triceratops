using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.DockerService.Managers.ClientManager;
using Triceratops.DockerService.Models;
using Triceratops.Libraries.Helpers;

namespace Triceratops.DockerService.Helpers
{
    public static class DockerRetryHelper
    {
        public static async Task<bool> WaitForStorageContainerAsync(TemporaryStorageContainer container, ILogger logger)
        {
            return await RetryHelper.RetryTask(async r =>
            {
                if (await container.CheckContainerAliveAsync())
                {
                    return true;
                }

                logger.LogInformation(
                    $"Waiting for temporary storage container {container.DockerContainerId} to be accessible (retry {r})"
                );

                return false;
            }, 30, TimeSpan.FromSeconds(1));
        }

        public static async Task<bool> WaitForContainerToStartAsync(
            string containerId,
            DockerClientManager clientManager,
            ILogger logger
        )
        {
            return await RetryHelper.RetryTask(async r =>
            {
                var response = await clientManager.InspectContainerAsync(containerId);

                if (response.DockerResponse.State.Running)
                {
                    return true;
                }

                logger.LogInformation($"Waiting for container {containerId} to start (retry {r})");

                return false;
            }, 30, TimeSpan.FromSeconds(1));
        }

        public static async Task<bool> WaitForContainerToStopAsync(
            string containerId,
            DockerClientManager clientManager,
            ILogger logger
        )
        {
            return await RetryHelper.RetryTask(async r =>
            {
                var response = await clientManager.InspectContainerAsync(containerId);

                if (!response.DockerResponse.State.Running)
                {
                    return true;
                }

                logger.LogInformation($"Waiting for container {containerId} to stop (retry {r})");

                return false;
            }, 30, TimeSpan.FromSeconds(1));
        }
    }
}
