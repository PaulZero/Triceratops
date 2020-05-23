using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Triceratops.Api.Services.DockerService.Interfaces;

namespace Triceratops.Api.Services.DockerService.Managers.ClientManager.Responses
{
    public class LoggedResponse : LoggedResponse<LoggedResponse>
    {
        public LoggedResponse(ILogger logger) : base(logger)
        {
        }
    }

    public abstract class LoggedResponse<TResponse> : IReadOnlyServiceResponse
        where TResponse : LoggedResponse<TResponse>
    {
        public bool Success { get; private set; }

        public IReadOnlyList<string> Errors => _errors.AsReadOnly();

        public IReadOnlyList<string> Warnings => _warnings.AsReadOnly();

        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _warnings = new List<string>();
        private readonly ILogger _logger;

        protected LoggedResponse(ILogger logger)
        {
            _logger = logger;
        }

        public TResponse LogException(Exception exception, string message)
        {
            _logger.LogError(exception, message);

            _errors.Add($"{message}: {exception.Message}");

            return (TResponse)this;
        }

        public TResponse LogError(string error)
        {
            _logger.LogError(error);

            _errors.Add(error);

            return (TResponse)this;
        }

        public TResponse LogWarning(string warning)
        {
            _logger.LogWarning(warning);

            _warnings.Add(warning);

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
