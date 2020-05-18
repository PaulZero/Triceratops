using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerOperationResponse : ITimedResponse
    {
        public bool Success { get; set; }

        [JsonIgnore]
        public TimeSpan Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public long DurationTicks
        {
            get => _duration.Ticks;
            set => _duration = TimeSpan.FromTicks(value);
        }

        public string Message { get; set; }

        private TimeSpan _duration;
    }
}
