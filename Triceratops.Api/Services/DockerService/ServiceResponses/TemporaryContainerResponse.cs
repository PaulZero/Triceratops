using MongoDB.Bson.IO;
using System.Collections.Generic;
using System.Linq;
using Triceratops.Api.Services.DockerService.Interfaces;
using Triceratops.Api.Services.DockerService.Models;

namespace Triceratops.Api.Services.DockerService.ServiceResponses
{
    public class TemporaryContainerResponse<TContainer> : DockerOperationResponse
        where TContainer : TemporaryContainer
    {
        public TContainer Container { get; }

        public TemporaryContainerResponse(IReadOnlyServiceResponse response) : base(response)
        {
        }

        public TemporaryContainerResponse(
            bool success,
            IEnumerable<string> errors,
            IEnumerable<string> warnings
        ) : base(success, errors, warnings)
        {
        }

        public TemporaryContainerResponse(
            bool success,
            IEnumerable<string> errors,
            IEnumerable<string> warnings,
            TContainer container
        ) : base(success, errors, warnings)
        {
            Container = container;
        }

        public TemporaryContainerResponse(TContainer container)
            : base(true, new string[0], new string[0])
        {                
            Container = container;
        }
    }
}
