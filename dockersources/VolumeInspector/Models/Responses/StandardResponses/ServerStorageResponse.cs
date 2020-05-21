using System.Collections.Generic;
using System.Net;
using Triceratops.VolumeInspector._Libraries;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.AbstractResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class ServerStorageResponse : AbstractJsonResponse
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.OK;

        public ServerStorageResponse(IEnumerable<VolumeDirectory> volumes)
            : base(VolumeInspectorResponseModel.CreateForObject(volumes))
        {
        }
    }
}