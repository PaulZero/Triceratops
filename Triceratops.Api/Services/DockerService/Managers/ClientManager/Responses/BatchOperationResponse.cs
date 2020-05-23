using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Triceratops.Api.Services.DockerService.Managers.ClientManager.Responses
{
    public class BatchOperationResponse : LoggedResponse<BatchOperationResponse>
    {
        public IReadOnlyList<string> FailedContainerIds => _failedContainerIds.AsReadOnly();

        public int FailureCount => _failedContainerIds.Count;

        public IReadOnlyList<string> SuccessfulContainerIds => _successfulContainerIds.AsReadOnly();

        public int SuccessCount => _successfulContainerIds.Count;

        public int TotalOperations => SuccessCount + FailureCount;

        private readonly List<string> _failedContainerIds = new List<string>();
        private readonly List<string> _successfulContainerIds = new List<string>();

        public BatchOperationResponse(ILogger logger) : base(logger)
        {
        }

        public void AddSuccessfulContainerId(string containerId)
        {
            _successfulContainerIds.Add(containerId);
        }

        public void AddFailedContainerId(string containerId)
        {
            _failedContainerIds.Add(containerId);
        }
    }
}
