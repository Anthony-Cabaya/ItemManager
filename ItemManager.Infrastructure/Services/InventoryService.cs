using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;

namespace ItemManager.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IItemStockRepository _stockRepo;
        private readonly ITransactionService _transactionService;
        private readonly IItemVariantRepository _itemVariantRepo;

        public InventoryService(
            IItemStockRepository stockRepo,
            ITransactionService transactionService,
            IItemVariantRepository itemVariantRepo)
        {
            _stockRepo = stockRepo;
            _transactionService = transactionService;
            _itemVariantRepo = itemVariantRepo;
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
            string updatedBy,
            int? itemVariantId = null)
        {
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative.");

            if (itemVariantId.HasValue)
            {
                var variant = await _itemVariantRepo
                    .GetByIdAsync(itemVariantId.Value);

                if (variant == null || variant.ItemID != itemId)
                {
                    throw new ArgumentException(
                        "Variant does not belong to the specified item.");
                }
            }

            var now = DateTime.Now;

            var model = new ItemStock
            {
                ItemID = itemId,
                LocationID = locationId,
                ItemVariantID = itemVariantId,
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

        public async Task StockInAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string username,
            string? notes = null,
            int? itemVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var now = DateTime.Now;

            var stock = await _stockRepo.GetByItemAndVariantAsync(
                itemId,
                locationId,
                itemVariantId);

            if (stock == null)
            {
                stock = new ItemStock
                {
                    ItemID = itemId,
                    LocationID = locationId,
                    ItemVariantID = itemVariantId,
                    Quantity = 0,
                    ReservedQuantity = 0,
                    CreatedBy = username,
                    CreatedDate = now,
                    UpdatedBy = username,
                    UpdatedDate = now,
                    LastUpdated = now
                };
            }

            stock.Quantity += quantity;
            stock.UpdatedBy = username;
            stock.UpdatedDate = now;
            stock.LastUpdated = now;

            await _stockRepo.UpsertAsync(stock);

            await _transactionService.StockInAsync(
                itemId,
                locationId,
                quantity,
                notes,
                username,
                itemVariantId);
        }

        public async Task StockOutAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string username,
            string? notes = null,
            int? itemVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var now = DateTime.Now;

            var stock = await _stockRepo.GetByItemAndVariantAsync(
                itemId,
                locationId,
                itemVariantId);

            if (stock == null || stock.Quantity < quantity)
                throw new InvalidOperationException("Insufficient stock.");

            stock.Quantity -= quantity;
            stock.UpdatedBy = username;
            stock.UpdatedDate = now;
            stock.LastUpdated = now;

            await _stockRepo.UpsertAsync(stock);

            await _transactionService.StockOutAsync(
                itemId,
                locationId,
                quantity,
                notes,
                username,
                itemVariantId);
        }

        public async Task HoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string username,
            string? notes = null,
            int? itemVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var now = DateTime.Now;

            var stock = await _stockRepo.GetByItemAndVariantAsync(
                itemId,
                locationId,
                itemVariantId);

            if (stock == null || stock.AvailableQuantity < quantity)
                throw new InvalidOperationException("Insufficient available stock.");

            stock.ReservedQuantity += quantity;
            stock.UpdatedBy = username;
            stock.UpdatedDate = now;
            stock.LastUpdated = now;

            await _stockRepo.UpsertAsync(stock);

            await _transactionService.HoldAsync(
                itemId,
                locationId,
                quantity,
                notes,
                username,
                itemVariantId);
        }

        public async Task ReleaseHoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string username,
            string? notes = null,
            int? itemVariantId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var now = DateTime.Now;

            var stock = await _stockRepo.GetByItemAndVariantAsync(
                itemId,
                locationId,
                itemVariantId);

            if (stock == null || stock.ReservedQuantity < quantity)
                throw new InvalidOperationException("Insufficient reserved stock.");

            stock.ReservedQuantity -= quantity;
            stock.UpdatedBy = username;
            stock.UpdatedDate = now;
            stock.LastUpdated = now;

            await _stockRepo.UpsertAsync(stock);

            await _transactionService.ReleaseHoldAsync(
                itemId,
                locationId,
                quantity,
                notes,
                username,
                itemVariantId);
        }

        public async Task<IEnumerable<ItemStock>> GetTotalStockPerItemAsync()
        {
            return await _stockRepo.GetTotalStockPerItemAsync();
        }

    }
}