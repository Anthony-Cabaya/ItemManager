using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class UnitCategoryRepository : IUnitCategoryRepository
    {
        private readonly DbHelper _dbHelper;

        public UnitCategoryRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Map Method
        private UnitCategory Map(SqlDataReader reader)
        {
            return new UnitCategory
            {
                UnitCategoryID = reader.GetInt32(reader.GetOrdinal("UnitCategoryID")),
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
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
                IsSystem = reader.GetBoolean(reader.GetOrdinal("IsSystem"))
            };
        }

        public async Task<List<UnitCategory>> GetAllAsync()
        {
            var list = new List<UnitCategory>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT UnitCategoryID, CategoryName, Sort,
                                     CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                                     IsSystem
                              FROM UnitCategory
                              ORDER BY Sort";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Unit Categories.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Unit Categories.", ex);
            }

            return list;
        }

        public async Task<UnitCategory?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT UnitCategoryID, CategoryName, Sort,
                                     CreatedBy, CreatedDate, UpdatedBy, UpdatedDate,
                                     IsSystem
                              FROM UnitCategory
                              WHERE UnitCategoryID = @UnitCategoryID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UnitCategoryID", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Map(reader);
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Unit Category by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Unit Category by ID.", ex);
            }

            return null;
        }

        public async Task AddAsync(UnitCategory model)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"INSERT INTO UnitCategory
                              (CategoryName, Sort, CreatedBy, CreatedDate)
                              VALUES
                              (@CategoryName, @Sort, @CreatedBy, @CreatedDate)";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                command.Parameters.AddWithValue("@Sort", model.Sort);
                command.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", (object?)model.CreatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while adding Unit Category.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding Unit Category.", ex);
            }
        }

        public async Task UpdateAsync(UnitCategory model)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"UPDATE UnitCategory
                              SET CategoryName = @CategoryName,
                                  Sort = @Sort,
                                  UpdatedBy = @UpdatedBy,
                                  UpdatedDate = @UpdatedDate
                              WHERE UnitCategoryID = @UnitCategoryID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UnitCategoryID", model.UnitCategoryID);
                command.Parameters.AddWithValue("@CategoryName", model.CategoryName);
                command.Parameters.AddWithValue("@Sort", model.Sort);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDate", (object?)model.UpdatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while updating Unit Category.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating Unit Category.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"DELETE FROM UnitCategory
                              WHERE UnitCategoryID = @UnitCategoryID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UnitCategoryID", id);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while deleting Unit Category.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting Unit Category.", ex);
            }
        }

        public async Task<int> GetUnitCountByCategoryAsync(int unitCategoryId)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"SELECT COUNT(*)
                              FROM Units
                              WHERE UnitCategoryID = @UnitCategoryID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UnitCategoryID", unitCategoryId);

                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while counting Units by Category.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while counting Units by Category.", ex);
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

                var parameterNames = idList
                    .Select((x, index) => $"@Id{index}")
                    .ToList();

                var query = $@"DELETE FROM UnitCategory
                               WHERE UnitCategoryID IN ({string.Join(", ", parameterNames)})";

                using var command = new SqlCommand(query, connection);

                for (int i = 0; i < idList.Count; i++)
                {
                    command.Parameters.AddWithValue(parameterNames[i], idList[i]);
                }

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while deleting Unit Categories.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting Unit Categories.", ex);
            }
        }

        public async Task<PagedResult<UnitCategory>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "")
        {
            try
            {
                var items = new List<UnitCategory>();

                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var countQuery = @"SELECT COUNT(*)
                                   FROM UnitCategory
                                   WHERE (@Search = ''
                                       OR CategoryName LIKE @SearchPattern)";

                using var countCommand = new SqlCommand(countQuery, connection);

                countCommand.Parameters.AddWithValue("@Search", search);
                countCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");

                var totalCount = Convert.ToInt32(
                    await countCommand.ExecuteScalarAsync());

                var offset = (pageNumber - 1) * pageSize;

                var dataQuery = @"SELECT UnitCategoryID,
                                         CategoryName,
                                         IsSystem,
                                         Sort,
                                         CreatedBy,
                                         CreatedDate,
                                         UpdatedBy,
                                         UpdatedDate
                                  FROM UnitCategory
                                  WHERE (@Search = ''
                                      OR CategoryName LIKE @SearchPattern)
                                  ORDER BY Sort ASC
                                  OFFSET @Offset ROWS
                                  FETCH NEXT @PageSize ROWS ONLY";

                using var dataCommand = new SqlCommand(dataQuery, connection);

                dataCommand.Parameters.AddWithValue("@Search", search);
                dataCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");
                dataCommand.Parameters.AddWithValue("@Offset", offset);
                dataCommand.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = await dataCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(Map(reader));
                }

                return new PagedResult<UnitCategory>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching paged Unit Categories.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching paged Unit Categories.", ex);
            }
        }

    }
}
