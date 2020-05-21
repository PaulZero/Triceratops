using System.Threading.Tasks;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class VerifyServerRunningRequest : AbstractRequest
    {
        protected override Task<IResponse> CreateResponseAsync()
        {
            return TaskResponse(new SuccessEmptyResponse());
        }
    }
}
