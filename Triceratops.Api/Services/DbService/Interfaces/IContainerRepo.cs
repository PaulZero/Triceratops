using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Triceratops.Api.Models;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IContainerRepo
    {
        Task<Container> FindByIdAsync(ObjectId id);

        Task<Container[]> FindByServerIdAsync(ObjectId stackId);

        Task SaveAsync(Container container);

        Task DeleteAsync(Container container);
    }
}
