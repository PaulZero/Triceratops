using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class DeleteFileRequest : AbstractRequest
    {
        protected override Task<IResponse> CreateResponseAsync()
        {
            try
            {
                var fileHash = GetRouteValue("fileHash");

                if (string.IsNullOrWhiteSpace(fileHash))
                {
                    return TaskResponse(new BadRequestResponse($"You cannot download a file without specifying a file hash."));
                }

                var fullPath = VolumeHelper.GetFullPathFromHash(fileHash, true);

                if (fullPath == null)
                {
                    return TaskResponse(new BadRequestResponse($"File hash could not be resolved to an existing file."));
                }

                File.Delete(fullPath);

                return TaskResponse(new SuccessMessageResponse("File successfully deleted"));
            }
            catch (Exception exception)
            {
                return TaskResponse(new ServerErrorResponse(exception, "Unable to delete file"));
            }
        }
    }
}
