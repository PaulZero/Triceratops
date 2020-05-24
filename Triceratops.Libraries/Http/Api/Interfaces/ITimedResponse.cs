using System;

namespace Triceratops.Libraries.Http.Api.Interfaces
{
    public interface ITimedResponse
    {
        public TimeSpan Duration { get; set; }
    }
}
