using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemUnitConversionRepository
    {
        Task<List<ItemUnitConversion>> GetByItemIdAsync(int itemId);
        Task<ItemUnitConversion?> GetByIdAsync(int conversionId);
        Task AddAsync(ItemUnitConversion model);
        Task DeleteAsync(int conversionId);
    }
}