using System;
using System.IO;

namespace Triceratops.Libraries.Http
{
    public class ReceivedFile : IDisposable
    {
        public string Name { get; }

        public Stream Stream { get; }

        public ReceivedFile(string name, Stream stream)
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
