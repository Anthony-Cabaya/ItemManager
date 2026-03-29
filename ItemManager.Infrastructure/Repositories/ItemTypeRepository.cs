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
    public class ItemTypeRepository : IItemTypeRepository
    {
        private readonly DbHelper _dbHelper;

        // Constructor
        public ItemTypeRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Show All Item Types
        public async Task<IEnumerable<ItemType>> GetAllAsync()
        {
            var itemTypes = new List<ItemType>();

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT ItemTypeID, ItemTypeName, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM ItemType ORDER BY Sort";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                itemTypes.Add(new ItemType
                {
                    ItemTypeID = reader.GetInt32(0),
                    ItemTypeName = reader.GetString(1),
                    Sort = reader.GetInt32(2),
                    CreatedBy = reader.GetString(3),
                    CreatedDate = reader.GetDateTime(4),
                    UpdatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    UpdatedDate = reader.IsDBNull(6) ? default : reader.GetDateTime(6)
                });
            }
            return itemTypes;
        }

        // Show only selected ID find Item Types
        public async Task<ItemType?> GetByIdAsync(int id)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = "SELECT ItemTypeID, ItemTypeName, Sort, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate FROM ItemType WHERE ItemTypeID = @ItemTypeID";
            
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ItemTypeID", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ItemType
                {
                    ItemTypeID = reader.GetInt32(0),
                    ItemTypeName = reader.GetString(1),
                    Sort = reader.GetInt32(2),
                    CreatedBy = reader.GetString(3),
                    CreatedDate = reader.GetDateTime(4),
                    UpdatedBy = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    UpdatedDate = reader.IsDBNull(6) ? default : reader.GetDateTime(6)
                };
            }
            return null;
        }

        // Insert another row in Table of Item Types
        public async Task AddAsync(ItemType itemType)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"INSERT INTO ItemType (ItemTypeName, Sort, CreatedBy, CreatedDate)
                          VALUES (@ItemTypeName, @Sort, @CreatedBy, @CreatedDate)";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemTypeName", itemType.ItemTypeName);
            command.Parameters.AddWithValue("@Sort", itemType.Sort);
            command.Parameters.AddWithValue("@CreatedBy", itemType.CreatedBy);
            command.Parameters.AddWithValue("@CreatedDate", itemType.CreatedDate);

            await command.ExecuteNonQueryAsync();
        }

        // Update selected row in Table of Item Types
        public async Task UpdateAsync(ItemType itemType)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"UPDATE ItemType
                          SET ItemTypeName = @ItemTypeName,
                              Sort = @Sort,
                              UpdatedBy = @UpdatedBy,
                              UpdatedDate = @UpdatedDate
                          WHERE ItemTypeID = @ItemTypeID";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemTypeID", itemType.ItemTypeID);
            command.Parameters.AddWithValue("@ItemTypeName", itemType.ItemTypeName);
            command.Parameters.AddWithValue("@Sort", itemType.Sort);
            command.Parameters.AddWithValue("@UpdatedBy", itemType.UpdatedBy);
            command.Parameters.AddWithValue("@UpdatedDate", itemType.UpdatedDate);

            await command.ExecuteNonQueryAsync();
        }
    }
}
