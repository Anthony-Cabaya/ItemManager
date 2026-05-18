using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();

        Task<Location?> GetByIdAsync(int id);

        Task AddAsync(Location model);

        Task UpdateAsync(Location model);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasStockAsync(int locationId);

        Task<PagedResult<Location>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "");
    }
}