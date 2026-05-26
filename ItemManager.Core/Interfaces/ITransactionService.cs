using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface ITransactionService
    {
        Task StockInAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy,
            int? itemVariantId = null);

        Task StockOutAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy,
            int? itemVariantId = null);

        Task HoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy,
            int? itemVariantId = null);

        Task ReleaseHoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy,
            int? itemVariantId = null);

        Task<IEnumerable<TransactionLog>> GetByItemAsync(int itemId);

        Task<IEnumerable<TransactionLog>> GetByLocationAsync(int locationId);

        Task<IEnumerable<TransactionLog>> GetRecentAsync(int count = 50);
    }
}