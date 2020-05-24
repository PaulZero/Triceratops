using System;
using System.Text.Json.Serialization;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerOperationResponse : AbstractEndpointResponse, ITimedResponse
    {
        public Guid ServerId { get; set; }

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

        private TimeSpan _duration;
    }
}
