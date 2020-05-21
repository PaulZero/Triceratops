using System;
using System.Net;
using Triceratops.VolumeInspector.Models.ResponseModels;
using Triceratops.VolumeInspector.Models.Responses.AbstractResponses;

namespace Triceratops.VolumeInspector.Models.Responses.StandardResponses
{
    internal class ServerErrorResponse : AbstractJsonResponse
    {
        public override HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

        public ServerErrorResponse(string error)
            : base(VolumeInspectorResponseModel.CreateForError(error))
        {
        }

        public ServerErrorResponse(Exception exception, string customError = null)
            : base(CreateResponseModel(exception, customError))
        {
        }

        private static VolumeInspectorResponseModel CreateResponseModel(Exception exception, string customError)
        {
            if (string.IsNullOrWhiteSpace(customError))
            {
                VolumeInspectorResponseModel.CreateForError($"An unhandled error occurred: {exception.Message}");
            }

            return VolumeInspectorResponseModel.CreateForError($"{customError}: {exception.Message}");
        }
    }
}
