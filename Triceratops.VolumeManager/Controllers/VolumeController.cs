using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Triceratops.VolumeManager.Models;
using Triceratops.VolumeManager.Services.StorageService.Interfaces;

namespace Triceratops.VolumeManager.Controllers
{
    public class VolumeController : Controller
    {
        private readonly IStorageService _storageService;

        public VolumeController(IStorageService storageService)
        {
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

        [HttpGet("/volumes/zip/{server}")]
        public IActionResult DownloadVolume(string server)
        {
            try
            {
                var stream = _storageService.GetServerZip(server);

                return File(stream, "application/zip", $"{server}.zip");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}