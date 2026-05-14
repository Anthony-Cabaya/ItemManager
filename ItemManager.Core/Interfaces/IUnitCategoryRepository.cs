using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IUnitCategoryRepository
    {
        Task<IEnumerable<UnitCategory>> GetAllAsync();

        Task<UnitCategory?> GetByIdAsync(int id);

        Task AddAsync(UnitCategory model);

        Task UpdateAsync(UnitCategory model);

        Task DeleteAsync(int id);

        Task<int> GetUnitCountByCategoryAsync(
            int unitCategoryId);

        Task<bool> HasUnitsAsync(
            int categoryId);

        Task DeleteManyAsync(
            IEnumerable<int> ids);

        Task<PagedResult<UnitCategory>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "");
    }
}