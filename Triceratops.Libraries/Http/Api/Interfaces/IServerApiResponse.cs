using System;

namespace Triceratops.Libraries.Http.Api.Interfaces
{
    public interface IServerApiResponse : IApiResponse
    {
        public Guid ServerId { get; set; }
    }
}
