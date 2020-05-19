using System;
using Triceratops.Libraries.Http.Api.Interfaces;

namespace Triceratops.Libraries.Http.Api.Exceptions
{
    public class ApiResponseFailureException : Exception
    {
        public Guid ServerId { get; }

        public bool HasServerId => ServerId != default;

        public ApiResponseFailureException(string errorMessage)
            : base(errorMessage)
        {
        }

        public ApiResponseFailureException(string errorPrefix, IApiResponse response)
            : base($"{errorPrefix}: {response.Error}")
        {
        }

        public ApiResponseFailureException(string errorPrefix, IServerApiResponse serverResponse)
            : this(errorPrefix, serverResponse as IApiResponse)
        {
            ServerId = serverResponse.ServerId;
        }
    }
}
