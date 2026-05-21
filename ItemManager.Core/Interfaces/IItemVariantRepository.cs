using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemVariantRepository
    {
        Task<List<ItemVariant>> GetByItemAsync(int itemId);

        Task<ItemVariant?> GetByIdAsync(int id);

        Task<int> AddAsync(ItemVariant variant, string username);

        Task UpdateAsync(ItemVariant variant, string username);

        Task DeleteAsync(int id);

        Task DeleteByItemAsync(int itemId);

        Task<bool> ExistsAsync(int itemId, string variantCode, int? excludeId = null);

        Task SetActiveAsync(int variantId, bool isActive, string username);

        Task<bool> HasStockAsync(int variantId);
    }
}