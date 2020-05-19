using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Http.Storage.Interfaces.Server;
using Triceratops.Libraries.Http.Storage.ResponseModels;
using Triceratops.Libraries.RouteMapping.Attributes;
using Triceratops.Libraries.RouteMapping.Enums;
using Triceratops.VolumeManager.Services.StorageService.Interfaces;

namespace Triceratops.VolumeManager.Controllers
{
    public class VolumeController : Controller, ITriceratopsStorageServer
    {
        private readonly ILogger _logger;

        private readonly IStorageService _storageService;

        public VolumeController(ILogger<VolumeController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [VolumeManagerRoute(VolumeManagerRoutes.GetServerNames)]
        public Task<ServerStorageNamesResponse> GetServerNamesAsync()
        {
            try
            {
                var serverNames = _storageService.GetServerStorageNames();

                return Task.FromResult(new ServerStorageNamesResponse
                {
                    ServerNames = serverNames
                });
            }
            catch
            {
                return Task.FromResult(ServerStorageNamesResponse.Empty);
            }
        }

        [VolumeManagerRoute(VolumeManagerRoutes.GetServer)]
        public Task<ServerStorageResponse> GetServerAsync(string serverSlug)
        {
            return Task.FromResult(new ServerStorageResponse
            {
                Server = _storageService.GetServerDetails(serverSlug)
            });
        }

        [VolumeManagerRoute(VolumeManagerRoutes.DownloadFile)]
        public async Task<IActionResult> DownloadFileAsync(string fileHash)
        {
            var relativePath = HashHelper.CreateString(fileHash);

            try
            {
                var stream = await _storageService.ReadFileAsync(relativePath);

                _logger.LogInformation($"Retrieved {stream.Stream.Length} byte download stream for {stream.FileName} ({stream.ContentType})");

                return File(stream.Stream, stream.ContentType, stream.FileName);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to generate stream for file {relativePath}: {exception.GetType().Name} - {exception.Message}");

                return BadRequest(exception.Message);
            }
        }

        [VolumeManagerRoute(VolumeManagerRoutes.UploadFile)]
        public async Task<bool> UploadFileAsync(string fileHash)
        {
            if (Request.ContentLength == 0)
            {
                throw new Exception("Empty content FFS.");
            }

            var relativePath = HashHelper.CreateString(fileHash);

            return await _storageService.WriteFileAsync(relativePath, Request.Body);
        }

        [VolumeManagerRoute(VolumeManagerRoutes.DownloadServerZip)]
        public async Task<IActionResult> DownloadServerZip(string serverSlug)
        {
            try
            {
                var stream = await _storageService.GetServerZipAsync(serverSlug);

                return File(stream, "application/zip", $"{serverSlug}.zip");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}