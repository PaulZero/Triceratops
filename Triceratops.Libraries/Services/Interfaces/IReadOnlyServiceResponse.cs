using System.Collections.Generic;

namespace Triceratops.Libraries.Services.Interfaces
{
    public interface IReadOnlyServiceResponse
    {
        IReadOnlyList<string> Errors { get; }

        bool Success { get; }

        IReadOnlyList<string> Warnings { get; }

        string ToJson();
    }
}
