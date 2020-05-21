using System;
using System.IO;
using System.Threading.Tasks;
using Triceratops.Libraries.Http.Api.ResponseModels;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Libraries.Http.Api.Interfaces.Client
{
    public interface ITriceratopsApiClient
    {
        IServerApiClient Servers { get; }

        IStorageApiClient Storage { get; }
    }
}
