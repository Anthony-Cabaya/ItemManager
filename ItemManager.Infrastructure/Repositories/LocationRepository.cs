using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly DbHelper _dbHelper;

        public LocationRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private const string BaseSelect = @"
            SELECT
                LocationID,
                LocationName,
                IsActive,
                Sort,
                CreatedBy,
                CreatedDate,
                UpdatedBy,
                UpdatedDate
            FROM Locations";

        private static Location Map(SqlDataReader reader)
        {
            return new Location
            {
                LocationID = reader.GetInt32(reader.GetOrdinal("LocationID")),
                LocationName = reader.GetString(reader.GetOrdinal("LocationName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
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
                    : reader.GetDateTime(reader.GetOrdinal("UpdatedDate"))
            };
        }

        private async Task<List<Location>> ExecuteQueryAsync(
            string query,
            Action<SqlCommand>? paramBuilder = null)
        {
            var list = new List<Location>();

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

        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            var query = $"{BaseSelect} ORDER BY Sort";
            return await ExecuteQueryAsync(query);
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            var query = $"{BaseSelect} WHERE LocationID = @LocationID";

            var result = await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@LocationID", id);
            });

            return result.FirstOrDefault();
        }

        public async Task AddAsync(Location model)
        {
            var query = @"
                INSERT INTO Locations
                (
                    LocationName,
                    IsActive,
                    Sort,
                    CreatedBy,
                    CreatedDate
                )
                VALUES
                (
                    @LocationName,
                    @IsActive,
                    @Sort,
                    @CreatedBy,
                    @CreatedDate
                )";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocationName", model.LocationName);
            command.Parameters.AddWithValue("@IsActive", model.IsActive);
            command.Parameters.AddWithValue("@Sort", model.Sort);
            command.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", (object?)model.CreatedDate ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Location model)
        {
            var query = @"
                UPDATE Locations
                SET
                    LocationName = @LocationName,
                    IsActive = @IsActive,
                    Sort = @Sort,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDate = @UpdatedDate
                WHERE LocationID = @LocationID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocationID", model.LocationID);
            command.Parameters.AddWithValue("@LocationName", model.LocationName);
            command.Parameters.AddWithValue("@IsActive", model.IsActive);
            command.Parameters.AddWithValue("@Sort", model.Sort);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedDate", (object?)model.UpdatedDate ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM Locations WHERE LocationID = @LocationID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocationID", id);

            var affected = await command.ExecuteNonQueryAsync();

            return affected > 0;
        }

        public async Task<bool> HasStockAsync(int locationId)
        {
            const string query = @"
                SELECT COUNT(1)
                FROM ItemStock
                WHERE LocationID = @LocationID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LocationID", locationId);

            var result = await command.ExecuteScalarAsync();

            return result != null && Convert.ToInt32(result) > 0;
        }

        public async Task<PagedResult<Location>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "")
        {
            var result = new PagedResult<Location>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            try
            {
                using var connection = _dbHelper.CreateConnection();

                await connection.OpenAsync();

                const string countQuery = @"
                    SELECT COUNT(*)
                    FROM Locations
                    WHERE (@Search = '' OR LocationName LIKE @SearchPattern)";

                using var countCommand = new SqlCommand(countQuery, connection);

                countCommand.Parameters.AddWithValue("@Search", search);
                countCommand.Parameters.AddWithValue("@SearchPattern", $"%{search}%");

                var countResult = await countCommand.ExecuteScalarAsync();

                result.TotalCount = countResult != null
                    ? Convert.ToInt32(countResult)
                    : 0;

                var offset = (pageNumber - 1) * pageSize;

                const string dataQuery = @"
                    SELECT
                        LocationID,
                        LocationName,
                        IsActive,
                        Sort,
                        CreatedBy,
                        CreatedDate,
                        UpdatedBy,
                        UpdatedDate
                    FROM Locations
                    WHERE (@Search = '' OR LocationName LIKE @SearchPattern)
                    ORDER BY Sort
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
                    result.Items.Add(Map(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("An error occurred while fetching paged Locations.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching paged Locations.", ex);
            }

            return result;
        }

    }
}