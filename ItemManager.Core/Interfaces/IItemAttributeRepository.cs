using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemAttributeRepository
    {
        Task<List<ItemAttribute>> GetByItemAsync(int itemId);

        Task<ItemAttribute?> GetByIdAsync(int id);

        Task<int> AddAsync(ItemAttribute attribute, string username);

        Task UpdateAsync(ItemAttribute attribute, string username);

        Task DeleteAsync(int id);

        Task DeleteByItemAsync(int itemId);
    }
}