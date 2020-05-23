namespace Triceratops.Api.Services.DockerService.Interfaces
{
    public interface IServiceResponse : IReadOnlyServiceResponse
    {
        void CopyFrom(IReadOnlyServiceResponse response);
    }
}
