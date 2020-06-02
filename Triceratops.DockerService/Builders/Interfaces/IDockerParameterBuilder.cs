using Microsoft.Extensions.Logging;

namespace Triceratops.DockerService.Builders.Interfaces
{
    interface IDockerParameterBuilder<T>
        where T : class, new()
    {
        void LogParameters(ILogger logger);

        T CreateParameters();
    }
}
