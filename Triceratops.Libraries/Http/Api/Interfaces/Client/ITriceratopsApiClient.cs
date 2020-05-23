namespace Triceratops.Libraries.Http.Api.Interfaces.Client
{
    public interface ITriceratopsApiClient
    {
        IServerApiClient Servers { get; }

        IStorageApiClient Storage { get; }
    }
}
