namespace Triceratops.Libraries.Services.Interfaces
{
    public interface IServiceResponse : IReadOnlyServiceResponse
    {
        void CopyFrom(IReadOnlyServiceResponse response);
    }
}
