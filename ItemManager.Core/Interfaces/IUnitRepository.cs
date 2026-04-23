using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAllAsync();

        Task<List<Unit>> GetByCategoryIdAsync(int categoryId);

        Task<Unit?> GetByIdAsync(int id);

        Task AddAsync(Unit model);

        Task UpdateAsync(Unit model);

        Task DeleteAsync(int id);

    }
}
