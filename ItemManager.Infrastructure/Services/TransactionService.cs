using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;

namespace ItemManager.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IItemStockRepository _stockRepo;
        private readonly ITransactionRepository _transactionRepo;

        public TransactionService(
            IItemStockRepository stockRepo,
            ITransactionRepository transactionRepo)
        {
            _stockRepo = stockRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task StockInAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            await _stockRepo.UpsertAsync(new ItemStock
            {
                ItemID = itemId,
                LocationID = locationId,
                Quantity = 0,
                ReservedQuantity = 0,
                LastUpdated = DateTime.Now,
                CreatedBy = createdBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = createdBy,
                UpdatedDate = DateTime.Now
            });

            await _stockRepo.UpdateQuantityAsync(
                itemId,
                locationId,
                quantity,
                createdBy);

            await _transactionRepo.AddAsync(
                BuildTransactionLog(
                    itemId,
                    locationId,
                    "StockIn",
                    quantity,
                    referenceNote,
                    createdBy));
        }

        public async Task StockOutAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            var stock = await _stockRepo.GetByItemAndLocationAsync(itemId, locationId);

            if (stock == null || stock.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Insufficient available stock.");
            }

            if ((stock.Quantity - quantity) < 0)
            {
                throw new InvalidOperationException("Quantity cannot go below zero.");
            }

            await _stockRepo.UpdateQuantityAsync(
                itemId,
                locationId,
                -quantity,
                createdBy);

            await _transactionRepo.AddAsync(
                BuildTransactionLog(
                    itemId,
                    locationId,
                    "StockOut",
                    quantity,
                    referenceNote,
                    createdBy));
        }

        public async Task HoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            var stock = await _stockRepo.GetByItemAndLocationAsync(itemId, locationId);

            if (stock == null || stock.AvailableQuantity < quantity)
            {
                throw new InvalidOperationException("Insufficient available stock to hold.");
            }

            await _stockRepo.UpdateReservedQuantityAsync(
                itemId,
                locationId,
                quantity,
                createdBy);

            await _transactionRepo.AddAsync(
                BuildTransactionLog(
                    itemId,
                    locationId,
                    "Hold",
                    quantity,
                    referenceNote,
                    createdBy));
        }

        public async Task ReleaseHoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            var stock = await _stockRepo.GetByItemAndLocationAsync(itemId, locationId);

            if (stock == null || stock.ReservedQuantity < quantity)
            {
                throw new InvalidOperationException("Insufficient reserved stock to release.");
            }

            if ((stock.ReservedQuantity - quantity) < 0)
            {
                throw new InvalidOperationException("Reserved quantity cannot go below zero.");
            }

            await _stockRepo.UpdateReservedQuantityAsync(
                itemId,
                locationId,
                -quantity,
                createdBy);

            await _transactionRepo.AddAsync(
                BuildTransactionLog(
                    itemId,
                    locationId,
                    "ReleaseHold",
                    quantity,
                    referenceNote,
                    createdBy));
        }

        public async Task<IEnumerable<TransactionLog>> GetByItemAsync(int itemId)
        {
            return await _transactionRepo.GetByItemAsync(itemId);
        }

        public async Task<IEnumerable<TransactionLog>> GetByLocationAsync(int locationId)
        {
            return await _transactionRepo.GetByLocationAsync(locationId);
        }

        public async Task<IEnumerable<TransactionLog>> GetRecentAsync(int count = 50)
        {
            return await _transactionRepo.GetRecentAsync(count);
        }

        private static TransactionLog BuildTransactionLog(
            int itemId,
            int locationId,
            string type,
            decimal quantity,
            string? note,
            string createdBy)
        {
            var now = DateTime.Now;

            return new TransactionLog
            {
                ItemID = itemId,
                LocationID = locationId,
                TransactionType = type,
                Quantity = quantity,
                ReferenceNote = note,
                TransactionDate = now,
                CreatedBy = createdBy,
                CreatedDate = now
            };
        }

    }
}