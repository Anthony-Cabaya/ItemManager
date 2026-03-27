using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IItemTypeRepository
    {
        Task<IEnumerable<ItemType>> GetAllAsync();
        Task<ItemType?> GetByIdAsync(int id);
        Task AddAsync(ItemType itemType);
        Task UpdateAsync(ItemType itemType);
    }
}
