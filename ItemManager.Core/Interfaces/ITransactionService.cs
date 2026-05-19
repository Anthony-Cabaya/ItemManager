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
            string createdBy);

        Task StockOutAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy);

        Task HoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy);

        Task ReleaseHoldAsync(
            int itemId,
            int locationId,
            decimal quantity,
            string? referenceNote,
            string createdBy);

        Task<IEnumerable<TransactionLog>> GetByItemAsync(int itemId);

        Task<IEnumerable<TransactionLog>> GetByLocationAsync(int locationId);

        Task<IEnumerable<TransactionLog>> GetRecentAsync(int count = 50);
    }
}