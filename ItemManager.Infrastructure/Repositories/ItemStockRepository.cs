using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemStockRepository : IItemStockRepository
    {
        private readonly DbHelper _dbHelper;

        public ItemStockRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private const string BaseSelect = @"
            SELECT
                s.StockID,
                s.ItemID,
                s.LocationID,
                s.ItemVariantID,
                s.Quantity,
                s.ReservedQuantity,
                s.MinStock,
                s.LastUpdated,
                s.CreatedBy,
                s.CreatedDate,
                s.UpdatedBy,
                s.UpdatedDate,
                i.ItemName,
                i.ItemCode,
                l.LocationName,
                iv.VariantName
            FROM ItemStock s
            INNER JOIN Items i ON s.ItemID = i.ItemID
            INNER JOIN Locations l ON s.LocationID = l.LocationID
            LEFT JOIN ItemVariants iv ON iv.ItemVariantID = s.ItemVariantID";

        private static ItemStock Map(SqlDataReader reader)
        {
            return new ItemStock
            {
                StockID = reader.GetInt32(reader.GetOrdinal("StockID")),

                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),

                LocationID = reader.GetInt32(reader.GetOrdinal("LocationID")),

                ItemVariantID = reader.IsDBNull(reader.GetOrdinal("ItemVariantID"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("ItemVariantID")),

                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),

                ReservedQuantity = reader.GetDecimal(reader.GetOrdinal("ReservedQuantity")),

                MinStock = reader.IsDBNull(reader.GetOrdinal("MinStock"))
                    ? null
                    : reader.GetDecimal(reader.GetOrdinal("MinStock")),

                LastUpdated = reader.IsDBNull(reader.GetOrdinal("LastUpdated"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("LastUpdated")),

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

                ItemName = reader.IsDBNull(reader.GetOrdinal("ItemName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ItemName")),

                ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ItemCode")),

                LocationName = reader.IsDBNull(reader.GetOrdinal("LocationName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("LocationName")),

                VariantName = reader.IsDBNull(reader.GetOrdinal("VariantName"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("VariantName"))
            };
        }

        private static ItemStock MapOverview(SqlDataReader reader)
        {
            return new ItemStock
            {
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode")) ? null : reader.GetString(reader.GetOrdinal("ItemCode")),
                ItemName = reader.IsDBNull(reader.GetOrdinal("ItemName")) ? null : reader.GetString(reader.GetOrdinal("ItemName")),
                BaseUnit = reader.IsDBNull(reader.GetOrdinal("BaseUnit")) ? null : reader.GetString(reader.GetOrdinal("BaseUnit")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity"))
            };
        }

        private async Task<List<ItemStock>> ExecuteQueryAsync(
            string query,
            Action<SqlCommand>? paramBuilder = null)
        {
            var list = new List<ItemStock>();

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

        public async Task<IEnumerable<ItemStock>> GetByItemAsync(int itemId)
        {
            var query = $@"
                {BaseSelect}
                WHERE s.ItemID = @ItemID
                ORDER BY l.LocationName";

            return await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
            });
        }

        public async Task<IEnumerable<ItemStock>> GetByLocationAsync(int locationId)
        {
            var query = $@"
                {BaseSelect}
                WHERE s.LocationID = @LocationID
                ORDER BY i.ItemName";

            return await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@LocationID", locationId);
            });
        }

        public async Task<ItemStock?> GetByItemAndLocationAsync(int itemId, int locationId)
        {
            var query = $@"
                {BaseSelect}
                WHERE s.ItemID = @ItemID
                AND s.LocationID = @LocationID
                AND s.ItemVariantID IS NULL";

            var result = await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
            });

            return result.FirstOrDefault();
        }

        public async Task<ItemStock?> GetByItemAndVariantAsync(
            int itemId,
            int locationId,
            int? variantId)
        {
            var query = $@"
                {BaseSelect}
                WHERE s.ItemID = @ItemID
                AND s.LocationID = @LocationID
                AND ((@VariantID IS NULL AND s.ItemVariantID IS NULL)
                    OR s.ItemVariantID = @VariantID)";

            var result = await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue(
                    "@VariantID",
                    (object?)variantId ?? DBNull.Value);
            });

            return result.FirstOrDefault();
        }

        public async Task UpsertAsync(ItemStock model)
        {
            const string query = @"
                MERGE ItemStock AS target
                USING
                (
                    SELECT
                        @ItemID AS ItemID,
                        @LocationID AS LocationID,
                        @ItemVariantID AS ItemVariantID
                ) AS source
                ON target.ItemID = source.ItemID
                AND target.LocationID = source.LocationID
                AND ((source.ItemVariantID IS NULL AND target.ItemVariantID IS NULL)
                    OR target.ItemVariantID = source.ItemVariantID
                )

                WHEN MATCHED THEN
                    UPDATE SET
                        Quantity = @Quantity,
                        MinStock = @MinStock,
                        LastUpdated = @LastUpdated,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = @UpdatedDate

                WHEN NOT MATCHED THEN
                    INSERT
                    (
                        ItemID,
                        LocationID,
                        ItemVariantID,
                        Quantity,
                        MinStock,
                        LastUpdated,
                        CreatedBy,
                        CreatedDate,
                        UpdatedBy,
                        UpdatedDate
                    )
                    VALUES
                    (
                        @ItemID,
                        @LocationID,
                        @ItemVariantID,
                        @Quantity,
                        @MinStock,
                        @LastUpdated,
                        @CreatedBy,
                        @CreatedDate,
                        @UpdatedBy,
                        @UpdatedDate
                    );";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ItemID", model.ItemID);
            command.Parameters.AddWithValue("@LocationID", model.LocationID);
            command.Parameters.AddWithValue("@ItemVariantID", (object?)model.ItemVariantID ?? DBNull.Value);
            command.Parameters.AddWithValue("@Quantity", model.Quantity);
            command.Parameters.AddWithValue("@MinStock", (object?)model.MinStock ?? DBNull.Value);
            command.Parameters.AddWithValue("@LastUpdated", (object?)model.LastUpdated ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedDate", (object?)model.CreatedDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@UpdatedDate", (object?)model.UpdatedDate ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<decimal> GetTotalStockAsync(int itemId)
        {
            const string query = @"
                SELECT COALESCE(SUM(Quantity), 0)
                FROM ItemStock
                WHERE ItemID = @ItemID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ItemID", itemId);

            var result = await command.ExecuteScalarAsync();

            return result != null ? Convert.ToDecimal(result) : 0;
        }

        public async Task<bool> DeleteAsync(int stockId)
        {
            const string query = @"
                DELETE FROM ItemStock
                WHERE StockID = @StockID";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@StockID", stockId);

            var affected = await command.ExecuteNonQueryAsync();

            return affected > 0;
        }

        public async Task<IEnumerable<ItemStock>> GetTotalStockPerItemAsync()
        {
            var sql = @"
                SELECT
                    i.ItemID,
                    i.ItemCode,
                    i.ItemName,
                    u.Abbreviation AS BaseUnit,
                    COALESCE(SUM(s.Quantity), 0) AS Quantity
                FROM Items i
                LEFT JOIN ItemStock s ON i.ItemID = s.ItemID
                LEFT JOIN Units u ON i.BaseUnitID = u.UnitID
                GROUP BY i.ItemID, i.ItemCode, i.ItemName, u.Abbreviation
                ORDER BY i.ItemName";

            return await ExecuteQueryAsync(sql, null, MapOverview);
        }

        private async Task<List<ItemStock>> ExecuteQueryAsync(
            string query,
            Action<SqlCommand>? paramBuilder,
            Func<SqlDataReader, ItemStock> mapper)
        {
            var list = new List<ItemStock>();

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                paramBuilder?.Invoke(command);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(mapper(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred.", ex);
            }

            return list;
        }

        public async Task UpdateQuantityAsync(
            int itemId,
            int locationId,
            decimal quantityDelta,
            string updatedBy,
            int? variantId = null)
        {
            const string query = @"
                UPDATE ItemStock
                SET
                    Quantity = Quantity + @Delta,
                    LastUpdated = @Now,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDate = @Now
                WHERE ItemID = @ItemID
                AND LocationID = @LocationID
                AND ((@VariantID IS NULL AND ItemVariantID IS NULL)
                    OR ItemVariantID = @VariantID)";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Delta", quantityDelta);
                command.Parameters.AddWithValue("@Now", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@ItemID", itemId);
                command.Parameters.AddWithValue("@LocationID", locationId);
                command.Parameters.AddWithValue("@VariantID", (object?)variantId ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while updating stock quantity.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating stock quantity.", ex);
            }
        }

        public async Task UpdateReservedQuantityAsync(
            int itemId,
            int locationId,
            decimal reservedDelta,
            string updatedBy,
            int? variantId = null)
        {
            const string query = @"
                UPDATE ItemStock
                SET
                    ReservedQuantity = ReservedQuantity + @Delta,
                    LastUpdated = @Now,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDate = @Now
                WHERE ItemID = @ItemID
                AND LocationID = @LocationID
                AND ((@VariantID IS NULL AND ItemVariantID IS NULL)
                    OR ItemVariantID = @VariantID)";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Delta", reservedDelta);
                command.Parameters.AddWithValue("@Now", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@ItemID", itemId);
                command.Parameters.AddWithValue("@LocationID", locationId);
                command.Parameters.AddWithValue("@VariantID", (object?)variantId ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while updating reserved stock quantity.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating reserved stock quantity.", ex);
            }
        }

    }
}