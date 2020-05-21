using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class UploadFileRequest : AbstractRequest
    {
        protected override async Task<IResponse> CreateResponseAsync()
        {
            try
            {
                var fileHash = GetRouteValue("fileHash");

                if (string.IsNullOrWhiteSpace(fileHash))
                {
                    return new BadRequestResponse($"You cannot download a file without specifying a file hash.");
                }

                var fullPath = VolumeHelper.GetFullPathFromHash(fileHash, true);

                using var readStream = GetRequestStream();

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                using var writeStream = File.OpenWrite(fullPath);
                await readStream.CopyToAsync(writeStream);

                return new SuccessMessageResponse($"File successfully uploaded");
            }
            catch (Exception exception)
            {
                return new ServerErrorResponse(exception, "File was not uploaded");
            }            
        }
    }
}
