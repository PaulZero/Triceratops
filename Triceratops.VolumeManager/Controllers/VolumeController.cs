using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Triceratops.Libraries.Helpers;
using Triceratops.Libraries.Models.Storage;
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

        [HttpGet("/servers")]
        public string[] ListServerNames()
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

        [HttpGet("/servers/{server}")]
        public ServerStorage GetServer(string server)
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

        [HttpGet("/servers/files/download/{fileHash}")]
        public async Task<IActionResult> DownloadFile(string fileHash)
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

        [HttpPost("/servers/files/upload/{fileHash}")]
        
        public async Task<IActionResult> UploadFile(string fileHash)
        {
            if (Request.ContentLength == 0)
            {
                throw new Exception("Empty content FFS.");
            }

            var relativePath = HashHelper.CreateString(fileHash);

            if (await _storageService.WriteFileAsync(relativePath, Request.Body))
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("/servers/{server}/zip")]
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
    }
}