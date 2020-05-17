using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;

namespace Triceratops.VolumeManager.Models
{
    public class DownloadStream
    {
        public string ContentType { get; }

        public string FileName { get; }

        public FileStream Stream { get; }

        public DownloadStream(FileStream stream)
        {
            ContentType = GetContentType(stream);
            FileName = GetFileName(stream);
            Stream = stream;            
        }

        private string GetFileName(FileStream stream)
        {
            try
            {
                return Path.GetFileName(stream.Name);
            }
            catch (Exception exception)
            {
                throw new IOException($"Unable to get file name from stream: {exception.Message}");
            }
        }

        private string GetContentType(FileStream stream)
        {
            try
            {
                new FileExtensionContentTypeProvider().TryGetContentType(stream.Name, out string contentType);

                return contentType ?? "application/octet-stream";
            }
            catch (Exception exception)
            {
                throw new IOException($"Unable to get content type from stream: {exception.Message}");
            }
        }
    }
}
