using System;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IDbService
    {
        IContainerRepo Containers { get; }

        IServerRepo Servers { get; }

        IVolumeRepo Volumes { get; }

        ITransaction StartTransaction();
    }
}
