using Triceratops.Libraries.Services;
using Triceratops.Libraries.Services.Interfaces;

namespace Triceratops.Api.Models.Responses.WebSockets
{
    public class WebSocketApiResponse : ReadOnlyServiceResponse, IServiceResponse
    {
        public void CopyFrom(IReadOnlyServiceResponse response)
        {
            Success = response.Success;

#if (DEBUG)
            // In debug mode, send the errors and warnings down to the frontend as well

            foreach (var error in response.Errors)
            {
                AddError(error);
            }

            foreach (var warning in response.Warnings)
            {
                AddWarning(warning);
            }
#endif
        }
    }
}
