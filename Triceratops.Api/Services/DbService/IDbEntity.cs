using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Triceratops.Api.Services.DbService
{
    public interface IDbEntity
    {
        uint Id { get; set; }
    }
}
