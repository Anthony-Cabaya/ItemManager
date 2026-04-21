using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

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

        private static Item Map(SqlDataReader reader)
        {
            return new Item
            {
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                ItemTypeID = reader.GetInt32(reader.GetOrdinal("ItemTypeID")),
                ItemSubTypeID = reader.IsDBNull(reader.GetOrdinal("ItemSubTypeID"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("ItemSubTypeID")),
                Sort = reader.GetInt32(reader.GetOrdinal("Sort")),

                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CreatedBy")),

                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),

                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("UpdatedBy")),

                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),

                ItemType = new ItemType
                {
                    ItemTypeName = reader.IsDBNull(reader.GetOrdinal("ItemTypeName"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("ItemTypeName"))
                },
                ItemSubTypeName = reader.IsDBNull(reader.GetOrdinal("SubTypeName"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("SubTypeName"))
            };
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var items = new List<Item>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.ItemSubTypeID, i.Sort, 
                           i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                           it.ItemTypeName,
                           ist.SubTypeName
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    LEFT JOIN ItemSubType ist ON i.ItemSubTypeID = ist.ItemSubTypeID
                    ORDER BY i.Sort";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(Map(reader));
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

                var query = @"
                    SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.ItemSubTypeID, i.Sort,
                           i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                           it.ItemTypeName,
                           ist.SubTypeName
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    LEFT JOIN ItemSubType ist ON i.ItemSubTypeID = ist.ItemSubTypeID
                    WHERE i.ItemID = @ItemID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Map(reader);
                }

                return null;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Item by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Item by ID.", ex);
            }
        }

        public async Task AddAsync(Item item)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO Items 
                    (ItemName, ItemTypeID, ItemSubTypeID, Sort, CreatedBy, CreatedDate)
                    VALUES 
                    (@ItemName, @ItemTypeID, @ItemSubTypeID, @Sort, @CreatedBy, @CreatedDate)";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemName", item.ItemName);
                command.Parameters.AddWithValue("@ItemTypeID", item.ItemTypeID);
                command.Parameters.AddWithValue("@ItemSubTypeID", (object?)item.ItemSubTypeID ?? DBNull.Value);
                command.Parameters.AddWithValue("@Sort", item.Sort);
                command.Parameters.AddWithValue("@CreatedBy", item.CreatedBy ?? (object)DBNull.Value);
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

                var query = @"
                    UPDATE Items
                    SET ItemName = @ItemName,
                        ItemTypeID = @ItemTypeID,
                        ItemSubTypeID = @ItemSubTypeID,
                        Sort = @Sort,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = @UpdatedDate
                    WHERE ItemID = @ItemID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemID", item.ItemID);
                command.Parameters.AddWithValue("@ItemName", item.ItemName);
                command.Parameters.AddWithValue("@ItemTypeID", item.ItemTypeID);
                command.Parameters.AddWithValue("@ItemSubTypeID", (object?)item.ItemSubTypeID ?? DBNull.Value);
                command.Parameters.AddWithValue("@Sort", item.Sort);
                command.Parameters.AddWithValue("@UpdatedBy", item.UpdatedBy ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDate", item.UpdatedDate ?? (object)DBNull.Value);

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
            int itemTypeFilter = 0,
            int itemSubTypeFilter = 0,
            bool includeAuditSearch = false)
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
                           CAST(i.Sort AS VARCHAR) LIKE @SearchPattern OR
                           (@IncludeAuditSearch = 1 AND i.CreatedBy LIKE @SearchPattern) OR
                           (@IncludeAuditSearch = 1 AND i.UpdatedBy LIKE @SearchPattern))
                           AND (@ItemTypeFilter = 0 OR i.ItemTypeID = @ItemTypeFilter)
                           AND (@ItemSubTypeFilter = 0 OR i.ItemSubTypeID = @ItemSubTypeFilter)";

                using var countCommand = new SqlCommand(countQuery, connection);
                countCommand.Parameters.AddWithValue("@Search", search);
                countCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                countCommand.Parameters.AddWithValue("@ItemTypeFilter", itemTypeFilter);
                countCommand.Parameters.AddWithValue("@IncludeAuditSearch", includeAuditSearch ? 1 : 0);
                countCommand.Parameters.AddWithValue("@ItemSubTypeFilter", itemSubTypeFilter);

                var countResult = await countCommand.ExecuteScalarAsync();
                result.TotalCount = countResult != null ? Convert.ToInt32(countResult) : 0;

                // Query 2 - get paged data
                var offset = (pageNumber - 1) * pageSize;

                var dataQuery = $@"
                    SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.ItemSubTypeID, i.Sort,
                           i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                           it.ItemTypeName,
                           ist.SubTypeName
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    LEFT JOIN ItemSubType ist ON i.ItemSubTypeID = ist.ItemSubTypeID
                    WHERE (@Search = '' OR 
                           i.ItemName LIKE @SearchPattern OR
                           CAST(i.Sort AS VARCHAR) LIKE @SearchPattern OR
                           (@IncludeAuditSearch = 1 AND i.CreatedBy LIKE @SearchPattern) OR
                           (@IncludeAuditSearch = 1 AND i.UpdatedBy LIKE @SearchPattern))
                           AND (@ItemTypeFilter = 0 OR i.ItemTypeID = @ItemTypeFilter)
                           AND (@ItemSubTypeFilter = 0 OR i.ItemSubTypeID = @ItemSubTypeFilter)
                    ORDER BY {sortColumn} {sortDirection}
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY";

                using var dataCommand = new SqlCommand(dataQuery, connection);
                dataCommand.Parameters.AddWithValue("@Search", search);
                dataCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                dataCommand.Parameters.AddWithValue("@ItemTypeFilter", itemTypeFilter);
                dataCommand.Parameters.AddWithValue("@IncludeAuditSearch", includeAuditSearch ? 1 : 0);
                dataCommand.Parameters.AddWithValue("@Offset", offset);
                dataCommand.Parameters.AddWithValue("@PageSize", pageSize);
                dataCommand.Parameters.AddWithValue("@ItemSubTypeFilter", itemSubTypeFilter);

                using var reader = await dataCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Items.Add(Map(reader));
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
