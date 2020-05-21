using System;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;
using Triceratops.VolumeInspector.Models.Responses;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class DownloadZipRequest : AbstractRequest
    {
        protected override Task<IResponse> CreateResponseAsync()
        {
            try
            {
                var zipStream = VolumeTreeHelper.CreateZip();

                return TaskResponse(new FileStreamResponse(zipStream, "server-volume.zip"));
            }
            catch (Exception exception)
            {
                return TaskResponse(new ServerErrorResponse(exception, "Unable to create zip file for server"));
            }            
        }
    }
}
