using System.Threading.Tasks;
using Triceratops.VolumeInspector.Models.Responses.Interfaces;
using Triceratops.VolumeInspector.Models.Responses.StandardResponses;

namespace Triceratops.VolumeInspector.Requests
{
    internal class MissingEndpointRequest : AbstractRequest
    {
        protected override Task<IResponse> CreateResponseAsync()
        {
            return Task.FromResult<IResponse>(new NotFoundResponse("The request could not be mapped to a specific endpoint"));
        }
    }
}
