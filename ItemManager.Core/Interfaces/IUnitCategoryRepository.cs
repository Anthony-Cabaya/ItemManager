using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IUnitCategoryRepository
    {
        Task<List<UnitCategory>> GetAllAsync();

        Task<UnitCategory?> GetByIdAsync(int id);

        Task AddAsync(UnitCategory model);

        Task UpdateAsync(UnitCategory model);

        Task DeleteAsync(int id);
    }
}
