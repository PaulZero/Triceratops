using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;

namespace Triceratops.Libraries.Models.Storage
{
    public class DownloadStream : IDisposable
    {
        public string ContentType { get; }

        public string FileName { get; }

        public Stream Stream { get; }

        public DownloadStream(FileStream stream)
        {
            ContentType = GetContentType(stream.Name);
            FileName = GetFileName(stream);
            Stream = stream;
        }

        public DownloadStream(Stream stream, string fileName)
        {
            ContentType = GetContentType(fileName);
            FileName = fileName;
            Stream = stream;
        }

        public string GetFileContents()
        {
            using var streamReader = new StreamReader(Stream);

            return streamReader.ReadToEnd();
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

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}