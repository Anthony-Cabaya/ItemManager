using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Interfaces;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly string _connectionString;

        public ItemRepository (string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
