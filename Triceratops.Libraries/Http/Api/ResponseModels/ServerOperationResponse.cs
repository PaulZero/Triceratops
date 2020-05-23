using System;
using System.Text.Json.Serialization;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerOperationResponse : ITimedResponse, IServerApiResponse
    {
        public Guid ServerId { get; set; }

        public bool Success { get; set; } = true;

        public string Error { get; set; }

        public bool IsRunning { get; set; }

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
