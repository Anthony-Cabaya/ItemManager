using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;

namespace ItemManager.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IItemStockRepository _stockRepo;

        public InventoryService(IItemStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<IEnumerable<ItemStock>> GetStockByItemAsync(int itemId)
        {
            return await _stockRepo.GetByItemAsync(itemId);
        }

        public async Task<IEnumerable<ItemStock>> GetStockByLocationAsync(int locationId)
        {
            return await _stockRepo.GetByLocationAsync(locationId);
        }

        public async Task<decimal> GetTotalStockAsync(int itemId)
        {
            return await _stockRepo.GetTotalStockAsync(itemId);
        }

        public async Task SetStockAsync(
            int itemId,
            int locationId,
            decimal quantity,
            decimal? minStock,
            string updatedBy)
        {
            if (quantity < 0)
            {
                throw new ArgumentException("Quantity cannot be negative.");
            }

            var now = DateTime.Now;

            var model = new ItemStock
            {
                ItemID = itemId,
                LocationID = locationId,
                Quantity = quantity,
                MinStock = minStock,
                LastUpdated = now,
                CreatedBy = updatedBy,
                CreatedDate = now,
                UpdatedBy = updatedBy,
                UpdatedDate = now
            };

            await _stockRepo.UpsertAsync(model);
        }

        public async Task<IEnumerable<ItemStock>> GetTotalStockPerItemAsync()
        {
            return await _stockRepo.GetTotalStockPerItemAsync();
        }

    }
}