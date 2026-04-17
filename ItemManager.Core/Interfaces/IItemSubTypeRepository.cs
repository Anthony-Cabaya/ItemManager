using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemSubTypeRepository
    {
        Task<IEnumerable<ItemSubType>> GetAllAsync();
        Task<IEnumerable<ItemSubType>> GetByItemTypeIdAsync(int itemTypeId);
        Task<ItemSubType?> GetByIdAsync(int id);
        Task AddAsync(ItemSubType itemSubType);
        Task UpdateAsync(ItemSubType itemSubType);
        Task DeleteAsync(int id);
    }
}
