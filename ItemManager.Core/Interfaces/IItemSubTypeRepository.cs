using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemSubTypeRepository
    {
        Task<IEnumerable<ItemSubType>> GetAllAsync();
        Task<IEnumerable<ItemSubType>> GetByItemTypeIdAsync(int itemTypeId);
        Task<ItemSubType?> GetByIdAsync(int id);
        Task AddAsync(ItemSubType itemSubType);
        Task UpdateAsync(ItemSubType itemSubType);
        Task DeleteAsync(int id);
        Task<int> GetItemCountBySubTypeAsync(int itemSubTypeId);
        Task DeleteManyAsync(IEnumerable<int> ids);
        Task<PagedResult<ItemSubType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            int itemTypeFilter = 0);
    }
}
