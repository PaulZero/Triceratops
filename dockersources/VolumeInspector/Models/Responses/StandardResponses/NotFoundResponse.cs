using System.Net;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.AbstractResponses;

namespace Triceratops.VolumeInspector.Models.Responses.StandardResponses
{
    internal class NotFoundResponse : AbstractJsonResponse
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public NotFoundResponse(string error) : base(VolumeInspectorResponseModel.CreateForError(error))
        {
        }
    }
}
