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

        // Base Select
        private const string BaseSelect = @"
            SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.ItemSubTypeID,
                   i.BaseUnitID, i.DisplayUnitID,
                   i.Sort,
                   i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                   it.ItemTypeName,
                   ist.SubTypeName,
                   bu.Abbreviation AS BaseUnitAbbreviation,
                   du.Abbreviation AS DisplayUnitAbbreviation
            FROM Items i
            INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
            LEFT JOIN ItemSubType ist ON i.ItemSubTypeID = ist.ItemSubTypeID
            LEFT JOIN Units bu ON i.BaseUnitID = bu.UnitID
            LEFT JOIN Units du ON i.DisplayUnitID = du.UnitID";

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

                BaseUnitID = reader.IsDBNull(reader.GetOrdinal("BaseUnitID"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("BaseUnitID")),

                DisplayUnitID = reader.IsDBNull(reader.GetOrdinal("DisplayUnitID"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("DisplayUnitID")),

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
                    : reader.GetString(reader.GetOrdinal("SubTypeName")),

                BaseUnitAbbreviation = reader.IsDBNull(reader.GetOrdinal("BaseUnitAbbreviation"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("BaseUnitAbbreviation")),

                DisplayUnitAbbreviation = reader.IsDBNull(reader.GetOrdinal("DisplayUnitAbbreviation"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("DisplayUnitAbbreviation")),
            };
        }

        // Generic Executor
        private async Task<List<Item>> ExecuteQueryAsync(string query, Action<SqlCommand>? paramBuilder = null)
        {
            var list = new List<Item>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                paramBuilder?.Invoke(command);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred.", ex);
            }

            return list;
        }

        public async Task<IEnumerable<Item>> GetAllAsync()
        {
            var query = $"{BaseSelect} ORDER BY i.Sort";
            return await ExecuteQueryAsync(query);
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            var query = $"{BaseSelect} WHERE i.ItemID = @ItemID";

            var result = await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemID", id);
            });

            return result.FirstOrDefault();
        }

        // Get By Type
        public async Task<IEnumerable<Item>> GetByItemTypeIdAsync(int itemTypeId)
        {
            var query = $"{BaseSelect} WHERE i.ItemTypeID = @ItemTypeID ORDER BY i.Sort";

            return await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemTypeID", itemTypeId);
            });
        }

        public async Task AddAsync(Item item)
        {
            var query = @"
                INSERT INTO Items
                (ItemName, ItemTypeID, ItemSubTypeID, BaseUnitID, DisplayUnitID, Sort, CreatedBy, CreatedDate)
                VALUES
                (@ItemName, @ItemTypeID, @ItemSubTypeID, @BaseUnitID, @DisplayUnitID, @Sort, @CreatedBy, @CreatedDate)";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemName", item.ItemName);
            command.Parameters.AddWithValue("@ItemTypeID", item.ItemTypeID);
            command.Parameters.AddWithValue("@ItemSubTypeID", (object?)item.ItemSubTypeID ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseUnitID", (object?)item.BaseUnitID ?? DBNull.Value);
            command.Parameters.AddWithValue("@DisplayUnitID", (object?)item.DisplayUnitID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Sort", item.Sort);
            command.Parameters.AddWithValue("@CreatedBy", (object?)item.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", (object?)item.CreatedDate ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Item item)
        {
            var query = @"
                UPDATE Items
                SET ItemName = @ItemName,
                    ItemTypeID = @ItemTypeID,
                    ItemSubTypeID = @ItemSubTypeID,
                    BaseUnitID = @BaseUnitID,
                    DisplayUnitID = @DisplayUnitID,
                    Sort = @Sort,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDate = @UpdatedDate
                WHERE ItemID = @ItemID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemID", item.ItemID);
            command.Parameters.AddWithValue("@ItemName", item.ItemName);
            command.Parameters.AddWithValue("@ItemTypeID", item.ItemTypeID);
            command.Parameters.AddWithValue("@ItemSubTypeID", (object?)item.ItemSubTypeID ?? DBNull.Value);
            command.Parameters.AddWithValue("@BaseUnitID", (object?)item.BaseUnitID ?? DBNull.Value);
            command.Parameters.AddWithValue("@DisplayUnitID", (object?)item.DisplayUnitID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Sort", item.Sort);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)item.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedDate", (object?)item.UpdatedDate ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var query = "DELETE FROM Items WHERE ItemID = @ItemID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ItemID", id);

            await command.ExecuteNonQueryAsync();
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
                    SELECT i.ItemID, i.ItemName, i.ItemTypeID, i.ItemSubTypeID,
                           i.BaseUnitID, i.DisplayUnitID,
                           i.Sort,
                           i.CreatedBy, i.CreatedDate, i.UpdatedBy, i.UpdatedDate,
                           it.ItemTypeName,
                           ist.SubTypeName,
                           bu.Abbreviation AS BaseUnitAbbreviation,
                           du.Abbreviation AS DisplayUnitAbbreviation
                    FROM Items i
                    INNER JOIN ItemType it ON i.ItemTypeID = it.ItemTypeID
                    LEFT JOIN ItemSubType ist ON i.ItemSubTypeID = ist.ItemSubTypeID
                    LEFT JOIN Units bu ON i.BaseUnitID = bu.UnitID
                    LEFT JOIN Units du ON i.DisplayUnitID = du.UnitID
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
