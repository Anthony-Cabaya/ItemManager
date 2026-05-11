using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemSubTypeRepository : IItemSubTypeRepository
    {
        private readonly DbHelper _dbHelper;

        public ItemSubTypeRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private static ItemSubType Map(SqlDataReader reader)
        {
            return new ItemSubType
            {
                ItemSubTypeID = reader.GetInt32(reader.GetOrdinal("ItemSubTypeID")),
                ItemTypeID = reader.GetInt32(reader.GetOrdinal("ItemTypeID")),
                ItemSubTypeName = reader.GetString(reader.GetOrdinal("SubTypeName")),
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

                ItemTypeName = reader.GetString(reader.GetOrdinal("ItemTypeName"))
            };
        }

        public async Task<IEnumerable<ItemSubType>> GetAllAsync()
        {
            var list = new List<ItemSubType>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT ist.ItemSubTypeID,
                                     ist.ItemTypeID,
                                     ist.SubTypeName,
                                     ist.Sort,
                                     ist.CreatedBy,
                                     ist.CreatedDate,
                                     ist.UpdatedBy,
                                     ist.UpdatedDate,
                                     it.ItemTypeName
                              FROM ItemSubType ist
                              INNER JOIN ItemType it ON ist.ItemTypeID = it.ItemTypeID
                              ORDER BY ist.Sort";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occured while fetching ItemSubTypes.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured while fetching ItemSubTypes.", ex);
            }

            return list;
        }

        public async Task<IEnumerable<ItemSubType>> GetByItemTypeIdAsync(int itemTypeId)
        {
            var list = new List<ItemSubType>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT ist.ItemSubTypeID,
                                     ist.ItemTypeID,
                                     ist.SubTypeName,
                                     ist.Sort,
                                     ist.CreatedBy,
                                     ist.CreatedDate,
                                     ist.UpdatedBy,
                                     ist.UpdatedDate,
                                     it.ItemTypeName
                              FROM ItemSubType ist
                              INNER JOIN ItemType it ON ist.ItemTypeID = it.ItemTypeID
                              WHERE ist.ItemTypeID = @ItemTypeID
                              ORDER BY ist.Sort";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemTypeID", itemTypeId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }

            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching ItemSubTypes by ItemTypeId.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching ItemSubTypes by ItemTypeId.", ex);
            }

            return list;
        }

        public async Task<ItemSubType?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT ist.ItemSubTypeID,
                                     ist.ItemTypeID,
                                     ist.SubTypeName,
                                     ist.Sort,
                                     ist.CreatedBy,
                                     ist.CreatedDate,
                                     ist.UpdatedBy,
                                     ist.UpdatedDate,
                                     it.ItemTypeName
                              FROM ItemSubType ist
                              INNER JOIN ItemType it ON ist.ItemTypeID = it.ItemTypeID
                              WHERE ist.ItemSubTypeID = @ItemSubTypeID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemSubTypeID", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Map(reader);
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching ItemSubType by Id.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching ItemSubType by Id.", ex);
            }

            return null;
        }

        public async Task AddAsync(ItemSubType itemSubType)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO ItemSubType
                              (ItemTypeID, SubTypeName, Sort, CreatedBy, CreatedDate)
                              VALUES
                              (@ItemTypeID, @SubTypeName, @Sort, @CreatedBy, @CreatedDate)";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemTypeID", itemSubType.ItemTypeID);
                command.Parameters.AddWithValue("@SubTypeName", itemSubType.ItemSubTypeName);
                command.Parameters.AddWithValue("@Sort", itemSubType.Sort);
                command.Parameters.AddWithValue("@CreatedBy", (object?)itemSubType.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", (object?)itemSubType.CreatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while adding ItemSubType.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding ItemSubType.", ex);
            }
        }

        public async Task UpdateAsync(ItemSubType itemSubType)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"UPDATE ItemSubType
                              SET ItemTypeID = @ItemTypeID,
                                  SubTypeName = @SubTypeName,
                                  Sort = @Sort,
                                  UpdatedBy = @UpdatedBy,
                                  UpdatedDate = @UpdatedDate
                              WHERE ItemSubTypeID = @ItemSubTypeID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemSubTypeID", itemSubType.ItemSubTypeID);
                command.Parameters.AddWithValue("@ItemTypeID", itemSubType.ItemTypeID);
                command.Parameters.AddWithValue("@SubTypeName", itemSubType.ItemSubTypeName);
                command.Parameters.AddWithValue("@Sort", itemSubType.Sort);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)itemSubType.UpdatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDate", (object?)itemSubType.UpdatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while updating ItemSubType.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating ItemSubType.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"DELETE FROM ItemSubType
                              WHERE ItemSubTypeID = @ItemSubTypeID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemSubTypeID", id);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while deleting ItemSubType.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting ItemSubType.", ex);
            }
        }

        public async Task<int> GetItemCountBySubTypeAsync(int itemSubTypeId)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT COUNT(*)
                      FROM Items
                      WHERE ItemSubTypeID = @ItemSubTypeID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue(
                    "@ItemSubTypeID",
                    itemSubTypeId);

                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    "An error occurred while counting Items by SubType.",
                    sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An unexpected error occurred while counting Items by SubType.",
                    ex);
            }
        }

        public async Task DeleteManyAsync(IEnumerable<int> ids)
        {
            try
            {
                var idList = ids.ToList();

                if (!idList.Any())
                    return;

                using var connection = _dbHelper.CreateConnection();

                await connection.OpenAsync();

                var parameters = idList
                    .Select((x, i) => $"@Id{i}")
                    .ToList();

                var query = $@"
            DELETE FROM ItemSubType
            WHERE ItemSubTypeID IN
            ({string.Join(",", parameters)})";

                using var command =
                    new SqlCommand(query, connection);

                for (int i = 0; i < idList.Count; i++)
                {
                    command.Parameters.AddWithValue(
                        parameters[i],
                        idList[i]);
                }

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    "An error occurred while deleting Sub Types.",
                    sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An unexpected error occurred while deleting Sub Types.",
                    ex);
            }
        }

        public async Task<PagedResult<ItemSubType>> GetPagedAsync(
    int pageNumber,
    int pageSize,
    string search = "",
    int itemTypeFilter = 0)
        {
            var items = new List<ItemSubType>();

            try
            {
                using var connection = _dbHelper.CreateConnection();

                await connection.OpenAsync();

                var countQuery = @"
            SELECT COUNT(*)
            FROM ItemSubType ist
            INNER JOIN ItemType it
                ON ist.ItemTypeID = it.ItemTypeID
            WHERE (@Search = ''
                OR ist.SubTypeName LIKE @SearchPattern)
            AND (@ItemTypeFilter = 0
                OR ist.ItemTypeID = @ItemTypeFilter)";

                using var countCommand =
                    new SqlCommand(countQuery, connection);

                countCommand.Parameters.AddWithValue(
                    "@Search",
                    search);

                countCommand.Parameters.AddWithValue(
                    "@SearchPattern",
                    $"%{search}%");

                countCommand.Parameters.AddWithValue(
                    "@ItemTypeFilter",
                    itemTypeFilter);

                var totalCount = Convert.ToInt32(
                    await countCommand.ExecuteScalarAsync());

                var dataQuery = @"
            SELECT ist.ItemSubTypeID,
                   ist.ItemTypeID,
                   ist.SubTypeName,
                   ist.Sort,
                   ist.CreatedBy,
                   ist.CreatedDate,
                   ist.UpdatedBy,
                   ist.UpdatedDate,
                   it.ItemTypeName
            FROM ItemSubType ist
            INNER JOIN ItemType it
                ON ist.ItemTypeID = it.ItemTypeID
            WHERE (@Search = ''
                OR ist.SubTypeName LIKE @SearchPattern)
            AND (@ItemTypeFilter = 0
                OR ist.ItemTypeID = @ItemTypeFilter)
            ORDER BY ist.Sort ASC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

                using var command =
                    new SqlCommand(dataQuery, connection);

                command.Parameters.AddWithValue(
                    "@Search",
                    search);

                command.Parameters.AddWithValue(
                    "@SearchPattern",
                    $"%{search}%");

                command.Parameters.AddWithValue(
                    "@ItemTypeFilter",
                    itemTypeFilter);

                command.Parameters.AddWithValue(
                    "@Offset",
                    (pageNumber - 1) * pageSize);

                command.Parameters.AddWithValue(
                    "@PageSize",
                    pageSize);

                using var reader =
                    await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(Map(reader));
                }

                return new PagedResult<ItemSubType>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(
                    "An error occurred while fetching paged Sub Types.",
                    sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "An unexpected error occurred while fetching paged Sub Types.",
                    ex);
            }
        }

    }
}
