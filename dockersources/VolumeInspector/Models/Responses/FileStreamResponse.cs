using System.IO;
using System.Net;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;

namespace Triceratops.VolumeInspector.Models.Responses
{
    internal class FileStreamResponse : IDownloadResponse
    {
        public string FileName { get; }

        public Stream Stream { get; }

        public HttpStatusCode StatusCode => HttpStatusCode.OK;

        public FileStreamResponse(FileStream fileStream)
        {
            FileName = fileStream.Name;
            Stream = fileStream;
        }

        public FileStreamResponse(Stream stream, string fileName)
        {
            FileName = fileName;
            Stream = stream;
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
