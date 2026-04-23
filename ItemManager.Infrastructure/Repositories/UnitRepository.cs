using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class UnitRepository : IUnitRepository
    {
        private readonly DbHelper _dbHelper;

        public UnitRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Map Method
        private Unit Map(SqlDataReader reader)
        {
            return new Unit
            {
                UnitID = reader.GetInt32(reader.GetOrdinal("UnitID")),
                UnitCategoryID = reader.GetInt32(reader.GetOrdinal("UnitCategoryID")),
                UnitName = reader.GetString(reader.GetOrdinal("UnitName")),
                Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation")),
                IsSystem = reader.GetBoolean(reader.GetOrdinal("IsSystem")),
                Sort = reader.GetInt32(reader.GetOrdinal("Sort")),

                // JOIN FIELD
                UnitCategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                                ? null
                                : reader.GetString(reader.GetOrdinal("CategoryName")),

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

        public async Task<List<Unit>> GetAllAsync()
        {
            var list = new List<Unit>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    SELECT u.UnitID,
                           u.UnitCategoryID,
                           u.UnitName,
                           u.Abbreviation,
                           u.IsSystem,
                           u.Sort,
                           uc.CategoryName,
                           u.CreatedBy,
                           u.CreatedDate,
                           u.UpdatedBy,
                           u.UpdatedDate
                    FROM Units u
                    INNER JOIN UnitCategory uc
                        ON u.UnitCategoryID = uc.UnitCategoryID
                    ORDER BY u.Sort";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Units.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Units.", ex);
            }

            return list;
        }

        public async Task<List<Unit>> GetByCategoryIdAsync(int categoryId)
        {
            var list = new List<Unit>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    SELECT u.UnitID,
                           u.UnitCategoryID,
                           u.UnitName,
                           u.Abbreviation,
                           u.IsSystem,
                           u.Sort,
                           uc.CategoryName,
                           u.CreatedBy,
                           u.CreatedDate,
                           u.UpdatedBy,
                           u.UpdatedDate
                    FROM Units u
                    INNER JOIN UnitCategory uc
                        ON u.UnitCategoryID = uc.UnitCategoryID
                    WHERE u.UnitCategoryID = @UnitCategoryID
                    ORDER BY u.Sort";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UnitCategoryID", categoryId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Units by Category.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Units by Category.", ex);
            }

            return list;
        }

        public async Task<Unit?> GetByIdAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    SELECT u.UnitID,
                           u.UnitCategoryID,
                           u.UnitName,
                           u.Abbreviation,
                           u.IsSystem,
                           u.Sort,
                           uc.CategoryName,
                           u.CreatedBy,
                           u.CreatedDate,
                           u.UpdatedBy,
                           u.UpdatedDate
                    FROM Units u
                    INNER JOIN UnitCategory uc
                        ON u.UnitCategoryID = uc.UnitCategoryID
                    WHERE u.UnitID = @UnitID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UnitID", id);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                    return Map(reader);
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while fetching Unit by ID.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while fetching Unit by ID.", ex);
            }

            return null;
        }

        public async Task AddAsync(Unit model)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    INSERT INTO Units
                    (UnitCategoryID, UnitName, Abbreviation, IsSystem, Sort,
                     CreatedBy, CreatedDate)
                    VALUES
                    (@UnitCategoryID, @UnitName, @Abbreviation, @IsSystem, @Sort,
                     @CreatedBy, @CreatedDate)";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UnitCategoryID", model.UnitCategoryID);
                command.Parameters.AddWithValue("@UnitName", model.UnitName);
                command.Parameters.AddWithValue("@Abbreviation", model.Abbreviation);
                command.Parameters.AddWithValue("@IsSystem", model.IsSystem);
                command.Parameters.AddWithValue("@Sort", model.Sort);
                command.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", (object?)model.CreatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while adding Unit.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding Unit.", ex);
            }
        }

        public async Task UpdateAsync(Unit model)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"
                    UPDATE Units
                    SET UnitCategoryID = @UnitCategoryID,
                        UnitName = @UnitName,
                        Abbreviation = @Abbreviation,
                        IsSystem = @IsSystem,
                        Sort = @Sort,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = @UpdatedDate
                    WHERE UnitID = @UnitID";

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UnitID", model.UnitID);
                command.Parameters.AddWithValue("@UnitCategoryID", model.UnitCategoryID);
                command.Parameters.AddWithValue("@UnitName", model.UnitName);
                command.Parameters.AddWithValue("@Abbreviation", model.Abbreviation);
                command.Parameters.AddWithValue("@IsSystem", model.IsSystem);
                command.Parameters.AddWithValue("@Sort", model.Sort);
                command.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDate", (object?)model.UpdatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while updating Unit.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating Unit.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                var query = @"DELETE FROM Units WHERE UnitID = @UnitID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UnitID", id);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("An error occurred while deleting Unit.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting Unit.", ex);
            }
        }

    }
}
