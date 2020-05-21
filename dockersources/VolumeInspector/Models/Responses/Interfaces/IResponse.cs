using System.Net;

namespace Triceratops.VolumeInspector.Models.Responses.Interfaces
{
    internal interface IResponse
    {
        HttpStatusCode StatusCode { get; }
    }
}
