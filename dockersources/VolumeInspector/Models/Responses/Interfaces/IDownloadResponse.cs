using System;
using System.IO;

namespace Triceratops.VolumeInspector.Models.Responses.Interfaces
{
    internal interface IDownloadResponse : IResponse, IDisposable
    {
        string FileName { get; }

        Stream Stream { get; }
    }
}
