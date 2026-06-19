using ItemManager.Core.Helpers;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(TransactionLog transaction);

        Task<IEnumerable<TransactionLog>> GetByItemAsync(int itemId);

        Task<IEnumerable<TransactionLog>> GetByLocationAsync(int locationId);

        Task<IEnumerable<TransactionLog>> GetRecentAsync(int count = 50);

    }
}