using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly DbHelper _dbHelper;

        // Constructor
        public ItemRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var items = new List<Item>();

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT ItemID, ItemName, ItemTypeID, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM Items ORDER BY Sort";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                items.Add(new Item
                {
                    ItemID = reader.GetInt32(0),
                    ItemName = reader.GetString(1),
                    ItemTypeID = reader.GetInt32(2),
                    Sort = reader.GetInt32(3),
                    CreatedBy = reader.GetString(4),
                    CreatedDate = reader.GetDateTime(5),
                    UpdatedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    UpdatedDate = reader.IsDBNull(7) ? default : reader.GetDateTime(7)
                });
            }
            return items;
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"SELECT ItemID, ItemName, ItemTypeID, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate 
                          FROM Items
                          WHERE ItemID = @ItemID";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ItemID", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Item
                {
                    ItemID = reader.GetInt32(0),
                    ItemName = reader.GetString(1),
                    ItemTypeID = reader.GetInt32(2),
                    Sort = reader.GetInt32(3),
                    CreatedBy = reader.GetString(4),
                    CreatedDate = reader.GetDateTime(5),
                    UpdatedBy = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    UpdatedDate = reader.IsDBNull(7) ? default : reader.GetDateTime(7)
                };
            }
            return null;
        }

        public async Task AddAsync(Item item)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"INSERT INTO Items (ItemName, ItemTypeID, Sort, CreatedBy, CreatedDate)
                          VALUES (@ItemName, @ItemTypeID, @Sort, @CreatedBy, @CreatedDate)";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemName", item.ItemName);
            command.Parameters.AddWithValue("@ItemTypeID", item.ItemTypeID);
            command.Parameters.AddWithValue("@Sort", item.Sort);
            command.Parameters.AddWithValue("@CreatedBy", item.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDate", item.CreatedDate);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = "DELETE FROM Items WHERE ItemID = @ItemID";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ItemID", id);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Item item)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"UPDATE Items
                          SET ItemName = @ItemName,
                              Sort = @Sort,
                              UpdatedBy = @UpdatedBy,
                              UpdatedDate = @UpdatedDate
                          WHERE ItemID = @ItemID";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemID", item.ItemID);
            command.Parameters.AddWithValue("@ItemName", item.ItemName);
            command.Parameters.AddWithValue("@Sort", item.Sort);
            command.Parameters.AddWithValue("@UpdatedBy", item.UpdatedBy);
            command.Parameters.AddWithValue("@UpdatedDate", item.UpdatedDate);

            await command.ExecuteNonQueryAsync();
        }
    }
}
