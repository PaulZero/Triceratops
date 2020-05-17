using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Triceratops.Libraries.Models.Storage;
using Triceratops.VolumeManager.Models;
using Triceratops.VolumeManager.Services.StorageService.Interfaces;

namespace Triceratops.VolumeManager.Controllers
{
    public class VolumeController : Controller
    {
        private readonly ILogger _logger;

        private readonly IStorageService _storageService;

        public VolumeController(ILogger<VolumeController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [HttpGet("/volumes")]
        public string[] List()
        {
            try
            {
                return _storageService.ListVolumes();
            }
            catch
            {
                return new string[0];
            }
        }

        [HttpGet("/volumes/list-entries/{server}")]
        public ServerInstance ListVolumeEntries(string server)
        {
            try
            {
                return _storageService.GetServerDetails(server);
            }
            catch
            {
                Response.StatusCode = 404;

                return null;
            }
        }

        [HttpGet("/volumes/files/download")]
        public IActionResult DownloadFile(string relativePath)
        {
            try
            {
                var stream = _storageService.GetFileStream(relativePath);

                _logger.LogInformation($"Retrieved {stream.Stream.Length} byte download stream for {stream.FileName} ({stream.ContentType})");

                return File(stream.Stream, stream.ContentType, stream.FileName);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to generate stream for file {relativePath}: {exception.GetType().Name} - {exception.Message}");

                return BadRequest(exception.Message);
            }
        }

        [HttpGet("/volumes/zip/{server}")]
        public IActionResult DownloadVolume(string server)
        {
            try
            {
                var stream = _storageService.GetServerZip(server);

                return File(stream, "application/zip", $"{server}.zip");
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        private string GetContentType(FileStream stream)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(stream.Name, out string contentType);

            return contentType ?? "application/octet-stream";
        }
    }
}