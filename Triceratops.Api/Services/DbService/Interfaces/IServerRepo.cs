using MongoDB.Bson;
using System.Threading.Tasks;
using Triceratops.Api.Models;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IServerRepo
    {
        public Task<Server[]> FindAllAsync();

        /// <summary>
        /// Find a server by its ID, along with the containers below it.
        /// </summary>
        public Task<Server> FindByIdAsync(ObjectId id);

        /// <summary>
        /// Save the specified server - This should do a deep save, and write the containers linked as well.
        /// </summary>
        public Task SaveAsync(Server server);

        /// <summary>
        /// Delete the specified server - This should do a deep delete, and destroy the containers linked as well.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public Task DeleteAsync(Server server);
    }
}
