using Newtonsoft.Json;
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

        /// <summary>
        /// The type of IStackConfiguration to be created from this row.
        /// </summary>
        public Type StackConfigurationType { get; set; }

        public async Task<Container[]> GetContainersAsync(IDbContainerRepo containerRepo)
        {
            return await containerRepo.FetchByStackId(Id);
        }
    }
}
