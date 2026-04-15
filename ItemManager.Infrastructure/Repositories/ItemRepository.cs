using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemManager.Core.Helpers;
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

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.Sort, 
                                 i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                                 it.ItemTypeName
                          FROM Items i
                          INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                          ORDER BY i.Sort";

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
                        UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                        UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                                        ? null
                                        : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                        ItemType = new ItemType
                        {
                            ItemTypeName = reader.GetString(8)
                        }
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Items.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Items.", ex);
            }
            
            return items;
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            try
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
                        UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                        UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                                        ? null
                                        : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
                    };
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Item by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Item by ID.", ex);
            }
            return null;
        }

        public async Task AddAsync(Item item)
        {
            try
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
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while adding Item.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding Item.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = "DELETE FROM Items WHERE ItemID = @ItemID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", id);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while deleting Item.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting Item.", ex);
            }
        }

        public async Task UpdateAsync(Item item)
        {
            try
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
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while updating Item.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating Item.", ex);
            }
        }

        // Pagination in Item
        public async Task<PagedResult<Item>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            string sortColumn = "Sort",
            string sortDirection = "asc",
            int itemTypeFilter = 0)
        {
            var result = new PagedResult<Item>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {
                // Whitelist sort column and direction
                var allowedColumns = new[] { "ItemName", "Sort", "ItemTypeName" };
                if (!allowedColumns.Contains(sortColumn))
                    sortColumn = "Sort";

                var allowedDirections = new[] { "asc", "desc" };
                if (!allowedDirections.Contains(sortDirection))
                    sortDirection = "asc";

                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                // Query 1 - get total count
                var countQuery = @"
                    SELECT COUNT(*)
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    WHERE (@Search = '' OR 
                           i.ItemName LIKE @SearchPattern OR
                           CAST(i.Sort AS VARCHAR) LIKE @SearchPattern)
                    AND (@ItemTypeFilter = 0 OR i.ItemTypeID = @ItemTypeFilter)";

                using var countCommand = new SqlCommand(countQuery, connection);
                countCommand.Parameters.AddWithValue("@Search", search);
                countCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                countCommand.Parameters.AddWithValue("@ItemTypeFilter", itemTypeFilter);

                result.TotalCount = (int)await countCommand.ExecuteScalarAsync();

                // Query 2 - get paged data
                var offset = (pageNumber - 1) * pageSize;

                var dataQuery = $@"
                    SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.Sort,
                           i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                           it.ItemTypeName
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    WHERE (@Search = '' OR 
                           i.ItemName LIKE @SearchPattern OR
                           CAST(i.Sort AS VARCHAR) LIKE @SearchPattern)
                    AND (@ItemTypeFilter = 0 OR i.ItemTypeID = @ItemTypeFilter)
                    ORDER BY {sortColumn} {sortDirection}
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

                using var dataCommand = new SqlCommand(dataQuery, connection);
                dataCommand.Parameters.AddWithValue("@Search", search);
                dataCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                dataCommand.Parameters.AddWithValue("@ItemTypeFilter", itemTypeFilter);
                dataCommand.Parameters.AddWithValue("@Offset", offset);
                dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await dataCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Items.Add(new Item
                    {
                        ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                        ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                        ItemTypeID = reader.GetInt32(reader.GetOrdinal("ItemTypeID")),
                        Sort = reader.GetInt32(reader.GetOrdinal("Sort")),
                        CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                        CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                        UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                        UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                                        ? null
                                        : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),

                        ItemType = new ItemType
                        {
                            ItemTypeName = reader.GetString(reader.GetOrdinal("ItemTypeName"))
                        }
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while fetching paged Items.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured while fetching paged Items.", ex);
            }

            return result;
        }

    }
}
