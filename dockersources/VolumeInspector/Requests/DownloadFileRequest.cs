using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;
using Triceratops.VolumeInspector.Models.Responses;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class DownloadFileRequest : AbstractRequest
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

                var fileStream = File.OpenRead(fullPath);

                return TaskResponse(new FileStreamResponse(fileStream));

            }
            catch (Exception exception)
            {
                return TaskResponse(new ServerErrorResponse(exception, "Unable to download file"));
            }
        }
    }
}
