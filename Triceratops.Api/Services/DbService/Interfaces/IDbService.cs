namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IDbService
    {
        IContainerRepo Containers { get; }

        IServerRepo Servers { get; }
    }
}
