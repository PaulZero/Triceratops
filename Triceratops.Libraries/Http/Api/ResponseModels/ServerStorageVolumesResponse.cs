using System.Collections.Generic;
using Triceratops.Libraries.Models.Storage;

namespace Triceratops.Libraries.Http.Api.ResponseModels
{
    public class ServerStorageVolumesResponse
    {
        public IEnumerable<VolumeDirectory> Volumes { get; set; }
    }
}
