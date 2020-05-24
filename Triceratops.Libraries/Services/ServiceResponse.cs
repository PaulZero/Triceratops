using Triceratops.Libraries.Services.Interfaces;

namespace Triceratops.Libraries.Services
{
    public class ServiceResponse : ReadOnlyServiceResponse, IServiceResponse
    {
        public void CopyFrom(IReadOnlyServiceResponse response)
        {
            Success = response.Success;

            foreach (var error in response.Errors)
            {
                AddError(error);
            }

            foreach (var warning in response.Warnings)
            {
                AddWarning(warning);
            }
        }
    }
}
