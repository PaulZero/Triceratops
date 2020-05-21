using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.Api.Services.ServerService;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models;
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;

namespace Triceratops.Api.Controllers
{
    public class StorageController : Controller
    {
        private readonly ILogger<StorageController> _logger;

        private readonly IServerService _serverService;

        public StorageController(ILogger<StorageController> logger, IServerService serverService)
        {
            _logger = logger;
            _serverService = serverService;
        }

        [StorageApiRoute(StorageApiRoutes.GetServerVolumesById)]
        public async Task<IActionResult> GetServerVolumesByIdAsync(Guid serverId)
        {
            var server = await _serverService.GetServerByIdAsync(serverId);

            return await GetServerVolumesAsync(server);
        }

        [StorageApiRoute(StorageApiRoutes.GetServerVolumesBySlug)]
        public async Task<IActionResult> GetServerVolumesBySlugAsync(string serverSlug)
        {
            var server = await _serverService.GetServerBySlugAsync(serverSlug);

            return await GetServerVolumesAsync(server);
        }

        [StorageApiRoute(StorageApiRoutes.DownloadFileById)]
        public async Task<IActionResult> DownloadFileByIdAsync(Guid serverId, string fileHash)
        {
            var server = await _serverService.GetServerByIdAsync(serverId);

            return await DownloadFileAsync(server, fileHash);
        }

        [StorageApiRoute(StorageApiRoutes.DownloadFileBySlug)]
        public async Task<IActionResult> DownloadFileBySlugAsync(string serverSlug, string fileHash)
        {
            var server = await _serverService.GetServerBySlugAsync(serverSlug);

            return await DownloadFileAsync(server, fileHash);
        }

        [StorageApiRoute(StorageApiRoutes.DownloadServerZipById)]
        public async Task<IActionResult> DownloadServerZipByIdAsync(Guid serverId)
        {
            var server = await _serverService.GetServerByIdAsync(serverId);

            return await DownloadServerZip(server);
        }

        [StorageApiRoute(StorageApiRoutes.DownloadServerZipBySlug)]
        public async Task<IActionResult> DownloadServerZipBySlugAsync(string serverSlug)
        {
            var server = await _serverService.GetServerBySlugAsync(serverSlug);

            return await DownloadServerZip(server);
        }

        [StorageApiRoute(StorageApiRoutes.UploadFileById)]
        public async Task<IActionResult> UploadFileByIdAsync(Guid serverId, string fileHash)
        {
            var server = await _serverService.GetServerByIdAsync(serverId);

            return await UploadFileAsync(server, fileHash);
        }

        [StorageApiRoute(StorageApiRoutes.UploadFileBySlug)]
        public async Task<IActionResult> UploadFileBySlugAsync(string serverSlug, string fileHash)
        {
            var server = await _serverService.GetServerBySlugAsync(serverSlug);

            return await UploadFileAsync(server, fileHash);
        }

        [StorageApiRoute(StorageApiRoutes.DeleteFileById)]
        public async Task<IActionResult> DeleteFileByIdAsync(Guid serverId, string fileHash)
        {
            var server = await _serverService.GetServerByIdAsync(serverId);

            return await DeleteFileAsync(server, fileHash);
        }

        [StorageApiRoute(StorageApiRoutes.DeleteFileBySlug)]
        public async Task<IActionResult> DeleteFileBySlugAsync(string serverSlug, string fileHash)
        {
            var server = await _serverService.GetServerBySlugAsync(serverSlug);

            return await DeleteFileAsync(server, fileHash);
        }

        private async Task<IActionResult> GetServerVolumesAsync(Server server)
        {
            try
            {
                var storageContainer = await _serverService.GetStorageContainerAsync(server.Id);
                var volumes = await storageContainer.GetServerStorageAsync();

                return Json(new ServerStorageVolumesResponse
                {
                    Volumes = volumes
                });
            }
            catch (Exception exception)
            {
                return LogBadRequest($"Unable to retrieve volumes for server {server.Id}: {exception.Message}");
            }
        }

        private async Task<IActionResult> DownloadFileAsync(Server server, string fileHash)
        {
            try
            {
                var storageContainer = await _serverService.GetStorageContainerAsync(server.Id);

                var relativeFilePath = HashHelper.CreateString(fileHash);

                var downloadStream = await storageContainer.DownloadFileAsync(relativeFilePath);

                return File(downloadStream.Stream, downloadStream.ContentType, downloadStream.FileName);
            }
            catch (Exception exception)
            {
                return LogBadRequest($"Failed to download file {fileHash} for server {server.Id}: {exception.Message}");
            }
        }

        private async Task<IActionResult> DownloadServerZip(Server server)
        {
            try
            {
                var storageContainer = await _serverService.GetStorageContainerAsync(server.Id);

                var downloadStream = await storageContainer.DownloadServerZipAsync();

                return File(downloadStream.Stream, downloadStream.ContentType, downloadStream.FileName);
            }
            catch (Exception exception)
            {
                return LogBadRequest($"Unable to download zip for server {server.Id}: {exception.Message}");
            }
        }

        private async Task<IActionResult> UploadFileAsync(Server server, string fileHash)
        {
            try
            {
                var storageContainer = await _serverService.GetStorageContainerAsync(server.Id);
                var relativePath = HashHelper.CreateString(fileHash);

                await storageContainer.UploadFileAsync(relativePath, Request.Body);

                return LogOkRequest($"File with hash {fileHash} uploaded successfully.");
            }
            catch (Exception exception)
            {
                return LogBadRequest($"Unable to upload file for server {server.Id}: {exception.Message}");
            }
        }

        private async Task<IActionResult> DeleteFileAsync(Server server, string fileHash)
        {
            try
            {
                var storageContainer = await _serverService.GetStorageContainerAsync(server.Id);
                var relativePath = HashHelper.CreateString(fileHash);

                await storageContainer.DeleteFileAsync(relativePath);

                return LogOkRequest($"File with hash {fileHash} deleted successfully.");
            }
            catch (Exception exception)
            {
                return LogBadRequest($"Unable to delete file for server {server.Id}: {exception.Message}");
            }
        }

        private IActionResult LogOkRequest(string message)
        {
            _logger.LogInformation(message);

            return Ok(message);
        }

        private IActionResult LogBadRequest(string error)
        {
            _logger.LogError(error);

            return BadRequest(error);
        }
    }
}
