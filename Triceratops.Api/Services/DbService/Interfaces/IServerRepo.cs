using System;
using System.Threading.Tasks;
using Triceratops.Libraries.Models;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IServerRepo
    {
        Task<Server[]> FindAllAsync();

        /// <summary>
        /// Find a server by its ID, along with the containers below it.
        /// </summary>
        Task<Server> FindByIdAsync(Guid id);


        /// <summary>
        /// Find a server by its name, along with the containers below it.
        /// </summary>
        Task<Server> FindBySlugAsync(string slug);

        /// <summary>
        /// Save the specified server - This should do a deep save, and write the containers linked as well.
        /// </summary>
        Task SaveAsync(Server server);

        /// <summary>
        /// Delete the specified server - This should do a deep delete, and destroy the containers linked as well.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        Task DeleteAsync(Server server);
    }
}
