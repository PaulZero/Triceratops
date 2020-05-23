using System.Collections.Generic;

namespace Triceratops.Api.Services.DockerService.Interfaces
{
    public interface IReadOnlyServiceResponse
    {
        IReadOnlyList<string> Errors { get; }

        bool Success { get; }

        IReadOnlyList<string> Warnings { get; }
    }
}
