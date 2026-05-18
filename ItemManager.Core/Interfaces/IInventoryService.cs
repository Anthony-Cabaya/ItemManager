using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<ItemStock>> GetStockByItemAsync(int itemId);

        Task<IEnumerable<ItemStock>> GetStockByLocationAsync(int locationId);

        Task<IEnumerable<ItemStock>> GetTotalStockPerItemAsync();

        Task SetStockAsync(
            int itemId,
            int locationId,
            decimal quantity,
            decimal? minStock,
            string updatedBy);

        Task<decimal> GetTotalStockAsync(int itemId);
    }
}