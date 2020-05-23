using System.Collections.Generic;
using System.Linq;
using Triceratops.Api.Services.DockerService.Interfaces;

namespace Triceratops.Api.Services.DockerService.ServiceResponses
{
    public class DockerOperationResponse : IServiceResponse
    {                
        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();

        public bool Success { get; private set; }

        private readonly List<string> _errors = new List<string>();

        private readonly List<string> _warnings = new List<string>();

        public DockerOperationResponse(IReadOnlyServiceResponse response)
        {
            CopyFrom(response);
        }

        public DockerOperationResponse(bool success, IEnumerable<string> errors, IEnumerable<string> warnings)
        {
            Success = success;
            _errors.AddRange(errors);
            _warnings.AddRange(warnings);
        }

        public void CopyFrom(IReadOnlyServiceResponse response)
        {
            Success = response.Success;

            _errors.Clear();
            _errors.AddRange(response.Errors);

            _warnings.Clear();
            _warnings.AddRange(response.Warnings);
        }
    }
}
