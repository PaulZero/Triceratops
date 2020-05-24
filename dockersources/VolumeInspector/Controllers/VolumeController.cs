using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;

namespace Triceratops.VolumeInspector.Controllers
{
    public class VolumeController : Controller
    {
        [HttpGet("/")]
        public IActionResult ListVolumes()
        {
            try
            {
                return Json(VolumeTreeHelper.BuildAll());
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/delete/{fileHash}")]
        public IActionResult DeleteFile(string fileHash)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(fileHash))
                {
                    return BadRequest();
                }

                var fullPath = VolumeHelper.GetFullPathFromHash(fileHash, true);

                if (fullPath == null)
                {
                    return BadRequest();
                }

                System.IO.File.Delete(fullPath);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/download/{fileHash}")]
        public IActionResult DownloadFileAsync(string fileHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileHash))
                {
                    return BadRequest();
                }

                var fullPath = VolumeHelper.GetFullPathFromHash(fileHash, true);

                if (fullPath == null)
                {
                    return BadRequest();
                }

                return File(fullPath, GetContentType(fullPath));
            }
            catch
            {
                return BadRequest();
            }

        }

        [HttpGet("/download-zip")]
        public IActionResult DownloadZipAsync()
        {
            try
            {
                return File(VolumeTreeHelper.CreateZip(), "application/zip");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("/upload/{fileHash}")]
        public async Task<IActionResult> UploadFileAsync(string fileHash)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileHash))
                {
                    return BadRequest();
                }

                var fullPath = VolumeHelper.GetFullPathFromHash(fileHash, true);

                using var readStream = Request.Body;

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                using var writeStream = System.IO.File.OpenWrite(fullPath);
                await readStream.CopyToAsync(writeStream);

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("/verify")]
        public IActionResult VerifyRunning()
        {
            return Ok();
        }

        private string GetContentType(string fileName)
        {
            try
            {
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);

                return contentType ?? "application/octet-stream";
            }
            catch (Exception exception)
            {
                throw new IOException($"Unable to get content type from stream: {exception.Message}");
            }
        }
    }
}
