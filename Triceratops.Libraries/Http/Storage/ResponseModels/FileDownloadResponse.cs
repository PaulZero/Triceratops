using System;
using System.IO;

namespace Triceratops.Libraries.Http.Storage.ResponseModels
{
    public class FileDownloadResponse : IDisposable
    {
        public string Name { get; }

        public Stream Stream { get; }

        public FileDownloadResponse(string name, Stream stream)
        {
            Name = name;
            Stream = stream;
        }

        public string GetStreamAsString()
        {
            using var reader = new StreamReader(Stream);

            return reader.ReadToEnd();
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
