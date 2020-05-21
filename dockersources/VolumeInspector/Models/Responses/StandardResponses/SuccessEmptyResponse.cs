using System.Net;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;

namespace Triceratops.VolumeInspector.Models.Responses.StandardResponses
{
    internal class SuccessEmptyResponse : IEmptyResponse
    {
        public HttpStatusCode StatusCode => HttpStatusCode.OK;
    }
}
