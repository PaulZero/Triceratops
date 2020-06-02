using System.IO;
using Triceratops.Libraries.Services;

namespace Triceratops.DockerService.ServiceResponses
{
    public class DockerStreamResponse : ReadOnlyServiceResponse
    {
        public Stream Stream { get; }

        public DockerStreamResponse()
        {
            Success = false;
        }

        public DockerStreamResponse(Stream stream)
        {
            Success = true;
            Stream = stream;
        }
    }
}
