using System.Net;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.AbstractResponses;

namespace Triceratops.VolumeInspector.Models.Responses.StandardResponses
{
    internal class BadRequestResponse : AbstractJsonResponse
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;

        public BadRequestResponse(string error)
            : base(VolumeInspectorResponseModel.CreateForError(error))
        {
        }
    }
}
