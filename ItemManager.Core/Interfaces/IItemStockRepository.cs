using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemStockRepository
    {
        Task<IEnumerable<ItemStock>> GetByItemAsync(int itemId);

        Task<IEnumerable<ItemStock>> GetByLocationAsync(int locationId);

        Task<IEnumerable<ItemStock>> GetTotalStockPerItemAsync();

        Task<ItemStock?> GetByItemAndLocationAsync(
            int itemId,
            int locationId);

        Task UpsertAsync(ItemStock model);

        Task<decimal> GetTotalStockAsync(int itemId);

        Task<bool> DeleteAsync(int stockId);

        Task UpdateQuantityAsync(
            int itemId,
            int locationId,
            decimal quantityDelta,
            string updatedBy,
            int? variantId = null);

        Task UpdateReservedQuantityAsync(
            int itemId,
            int locationId,
            decimal reservedDelta,
            string updatedBy,
            int? variantId = null);

        Task<ItemStock?> GetByItemAndVariantAsync(
            int itemId,
            int locationId,
            int? variantId);
    }
}