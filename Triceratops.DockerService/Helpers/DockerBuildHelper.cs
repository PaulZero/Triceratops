using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.DockerService.Helpers
{
    public class DockerBuildHelper
    {
        public static async Task<bool> ValidateBuildAsync(Stream buildResponseStream, ILogger logger)
        {
            using var loggerScope = logger.BeginScope("Validating response stream from Docker build");
            var lines = new List<string>();

            try
            {
                if (buildResponseStream == null)
                {
                    return false;
                }

                using var reader = new StreamReader(buildResponseStream);

                do
                {
                    var line = await reader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    lines.Add(line);
                }
                while (!reader.EndOfStream);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
