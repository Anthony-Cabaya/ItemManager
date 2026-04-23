using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllAsync();
        Task<Item?> GetByIdAsync(int id);
        Task AddAsync(Item item);
        Task UpdateAsync(Item item);
        Task DeleteAsync(int id);
        Task<PagedResult<Item>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int itemTypeFilter = 0,
            int itemSubTypeFilter = 0,
            bool includeAuditSearch = false);
        Task<IEnumerable<Item>> GetByItemTypeIdAsync(int itemTypeId);
    }
}
