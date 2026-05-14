using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IUnitRepository
    {
        Task<IEnumerable<Unit>> GetAllAsync();

        Task<IEnumerable<Unit>> GetByCategoryIdAsync(int categoryId);

        Task<Unit?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(
            string name,
            int categoryId,
            int? excludeId = null);

        Task<int> CreateAsync(Unit unit);

        Task<bool> UpdateAsync(Unit unit);

        Task<bool> DeleteAsync(int id);

        Task<bool> HasItemsUsingUnitAsync(
            int unitId);

    }
}
