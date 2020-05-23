using System;
using System.Threading.Tasks;
using Triceratops.VolumeInspector.Helpers;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class VolumeTreeRequest : AbstractRequest
    {
        protected override Task<IResponse> CreateResponseAsync()
        {
            try
            {
                return Task.FromResult<IResponse>(new ServerStorageResponse(VolumeTreeHelper.BuildAll()));
            } 
            catch (Exception exception)
            {
                return Task.FromResult<IResponse>(new ServerErrorResponse(exception, "Unable to get list of server volumes"));
            }            
        }
    }
}
