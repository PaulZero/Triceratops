using Microsoft.Extensions.Logging;

namespace Triceratops.Api.Services.DockerService.Builders.Interfaces
{
    interface IDockerParameterBuilder<T>
        where T : class, new()
    {
        void LogParameters(ILogger logger);

        T CreateParameters();
    }
}
