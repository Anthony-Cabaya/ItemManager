using ItemManager.Core.Interfaces;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemCodeRepository : IItemCodeRepository
    {
        private readonly DbHelper _dbHelper;

        public ItemCodeRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<int> GetNextSequenceAsync(
            int itemTypeId,
            int? itemSubTypeId)
        {
            try
            {
                var query = @"
                    MERGE ItemCodeSequences AS target
                    USING (
                        SELECT @ItemTypeID AS ItemTypeID,
                               @ItemSubTypeID AS ItemSubTypeID
                    ) AS source
                    ON (
                        target.ItemTypeID = source.ItemTypeID
                        AND (
                            target.ItemSubTypeID = source.ItemSubTypeID
                            OR (
                                target.ItemSubTypeID IS NULL
                                AND source.ItemSubTypeID IS NULL
                            )
                        )
                    )
                    WHEN MATCHED THEN
                        UPDATE SET LastSequence = LastSequence + 1
                    WHEN NOT MATCHED THEN
                        INSERT (ItemTypeID, ItemSubTypeID, LastSequence)
                        VALUES (@ItemTypeID, @ItemSubTypeID, 1);

                    SELECT LastSequence
                    FROM ItemCodeSequences
                    WHERE ItemTypeID = @ItemTypeID
                    AND (
                        ItemSubTypeID = @ItemSubTypeID
                        OR (
                            ItemSubTypeID IS NULL
                            AND @ItemSubTypeID IS NULL
                        )
                    );";

                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemTypeID", itemTypeId);
                command.Parameters.AddWithValue(
                    "@ItemSubTypeID",
                    (object?)itemSubTypeId ?? DBNull.Value);

                var result = await command.ExecuteScalarAsync();

                return result != null
                    ? Convert.ToInt32(result)
                    : 1;
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "Database error occurred while generating item code sequence.",
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Unexpected error occurred while generating item code sequence.",
                    ex);
            }
        }

        public async Task<bool> IsCodeUniqueAsync(string itemCode)
        {
            try
            {
                const string query = @"
                            SELECT COUNT(*)
                            FROM Items
                            WHERE ItemCode = @ItemCode";

                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemCode", itemCode);

                var result = await command.ExecuteScalarAsync();

                var count = result != null
                    ? Convert.ToInt32(result)
                    : 0;

                return count == 0;
            }
            catch (SqlException ex)
            {
                throw new Exception(
                    "Database error occurred while checking item code uniqueness.",
                    ex);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Unexpected error occurred while checking item code uniqueness.",
                    ex);
            }
        }

        public async Task<int> PeekNextSequenceAsync(
    int itemTypeId, int? itemSubTypeId)
        {
            try
            {
                const string query = @"
                    SELECT ISNULL(LastSequence, 0) + 1
                    FROM ItemCodeSequences
                    WHERE ItemTypeID = @ItemTypeID
                    AND (ItemSubTypeID = @ItemSubTypeID
                         OR (ItemSubTypeID IS NULL
                             AND @ItemSubTypeID IS NULL))";

                using var conn = _dbHelper.CreateConnection();
                await conn.OpenAsync();

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue(
                    "@ItemTypeID", itemTypeId);
                cmd.Parameters.AddWithValue(
                    "@ItemSubTypeID",
                    (object?)itemSubTypeId ?? DBNull.Value);

                var result = await cmd.ExecuteScalarAsync();
                return result != null
                    ? Convert.ToInt32(result) : 1;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error peeking sequence.", ex);
            }
        }

    }
}
