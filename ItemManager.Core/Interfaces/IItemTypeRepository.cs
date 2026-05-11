using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemTypeRepository
    {
        Task<IEnumerable<ItemType>> GetAllAsync();
        Task<ItemType?> GetByIdAsync(int id);
        Task AddAsync(ItemType itemType);
        Task UpdateAsync(ItemType itemType);
        Task<int> GetItemCountByTypeAsync(int itemTypeId);
        Task DeleteManyAsync(IEnumerable<int> ids);
        Task<PagedResult<ItemType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            bool includeAuditSearch = false);
    }
}
