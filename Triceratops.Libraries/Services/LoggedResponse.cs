using Microsoft.Extensions.Logging;
using System;

namespace Triceratops.Libraries.Services
{
    public class LoggedResponse : LoggedResponse<LoggedResponse>
    {
        public LoggedResponse(ILogger logger) : base(logger)
        {
        }
    }

    public abstract class LoggedResponse<TResponse> : ReadOnlyServiceResponse
        where TResponse : LoggedResponse<TResponse>
    {
        private readonly ILogger _logger;

        protected LoggedResponse(ILogger logger)
        {
            _logger = logger;
        }

        public TResponse LogException(Exception exception, string message)
        {
            _logger.LogError(exception, message);

            AddError($"{message}: {exception.Message}");

            return (TResponse)this;
        }

        public TResponse LogError(string error)
        {
            _logger.LogError(error);

            AddError(error);

            return (TResponse)this;
        }

        public TResponse LogWarning(string warning)
        {
            _logger.LogWarning(warning);

            AddWarning(warning);

            return (TResponse)this;
        }

        public TResponse HasFailed()
        {
            Success = false;

            return (TResponse)this; ;
        }

        public TResponse IsSuccessful()
        {
            Success = true;

            return (TResponse)this;
        }
    }
}
