using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Services.DbService;

namespace Triceratops.Api.Models.Persistence.Stacks
{
    public class ContainerStack : IDbEntity
    {
        public uint Id { get; set; }

        public Type StackConfigurationType { get; set; }
    }
}
