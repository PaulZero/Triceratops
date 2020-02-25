using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DbService;

namespace Triceratops.Api.Models.Persistence.Stacks
{
    public class Container : IDbEntity
    {
        public uint Id { get; set; }

        public uint StackId { get; set; }

        public string ImageName { get; set; }
    }
}
