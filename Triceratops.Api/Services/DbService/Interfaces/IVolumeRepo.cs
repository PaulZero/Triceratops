using System;
using System.Threading.Tasks;
using Triceratops.Api.Models;
using Triceratops.Api.Services.DbService.Events;

namespace Triceratops.Api.Services.DbService.Interfaces
{
    public interface IVolumeRepo
    {
        Task<Volume[]> FindAllAsync();

        Task<Volume[]> FindByContainerIdAsync(Guid containerId);

        Task<Volume> FindByIdAsync(Guid volumeId);

        Task DeleteAsync(Volume volume);

        Task SaveAsync(Volume volume);
    }
}
