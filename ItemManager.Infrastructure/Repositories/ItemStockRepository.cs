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
                s.Quantity,
                s.MinStock,
                s.LastUpdated,
                s.CreatedBy,
                s.CreatedDate,
                s.UpdatedBy,
                s.UpdatedDate,
                i.ItemName,
                i.ItemCode,
                l.LocationName
            FROM ItemStock s
            INNER JOIN Items i ON s.ItemID = i.ItemID
            INNER JOIN Locations l ON s.LocationID = l.LocationID";

        private static ItemStock Map(SqlDataReader reader)
        {
            return new ItemStock
            {
                StockID = reader.GetInt32(reader.GetOrdinal("StockID")),
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                LocationID = reader.GetInt32(reader.GetOrdinal("LocationID")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),

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
                    : reader.GetString(reader.GetOrdinal("LocationName"))
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

        public async Task<ItemStock?> GetByItemAndLocationAsync(
            int itemId,
            int locationId)
        {
            var query = $@"
                {BaseSelect}
                WHERE s.ItemID = @ItemID
                AND s.LocationID = @LocationID";

            var result = await ExecuteQueryAsync(query, cmd =>
            {
                cmd.Parameters.AddWithValue("@ItemID", itemId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
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
                        @LocationID AS LocationID
                ) AS source
                ON target.ItemID = source.ItemID
                AND target.LocationID = source.LocationID

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

            return result != null
                ? Convert.ToDecimal(result)
                : 0;
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
    }
}