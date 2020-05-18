namespace Triceratops.Libraries.Http.Api.Interfaces.Client
{
    public interface ITriceratopsApiClient
    {
        ITriceratopsServersApiClient Servers { get; }
    }
}
