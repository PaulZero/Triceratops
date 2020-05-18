using System;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public interface ITimedResponse
    {
        public TimeSpan Duration { get; set; }
    }
}
