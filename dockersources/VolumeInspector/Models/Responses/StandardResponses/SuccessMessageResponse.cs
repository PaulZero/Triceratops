using System.Net;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.AbstractResponses;

namespace Triceratops.VolumeInspector.Models.Responses.StandardResponses
{
    internal class SuccessMessageResponse : AbstractJsonResponse
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.OK;

        public SuccessMessageResponse(string message)
            : base(VolumeInspectorResponseModel.CreateWithMessage(message))
        {
        }
    }
}
