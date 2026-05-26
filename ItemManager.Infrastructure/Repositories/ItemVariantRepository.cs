using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemVariantRepository : IItemVariantRepository
    {
        private readonly DbHelper _dbHelper;

        public ItemVariantRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private const string BaseSelect = @"
            SELECT v.ItemVariantID, v.ItemID, v.VariantCode, v.VariantName,
                   v.IsActive, v.Sort,
                   i.ItemName, i.ItemCode,
                   v.AttributesText
            FROM ItemVariants v
            INNER JOIN Items i ON i.ItemID = v.ItemID";

        private static ItemVariant Map(SqlDataReader reader)
        {
            return new ItemVariant
            {
                ItemVariantID = reader.GetInt32(reader.GetOrdinal("ItemVariantID")),
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                VariantCode = reader.GetString(reader.GetOrdinal("VariantCode")),
                VariantName = reader.GetString(reader.GetOrdinal("VariantName")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                Sort = reader.GetInt32(reader.GetOrdinal("Sort")),
                ItemName = reader.IsDBNull(reader.GetOrdinal("ItemName")) ? null : reader.GetString(reader.GetOrdinal("ItemName")),
                ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode")) ? null : reader.GetString(reader.GetOrdinal("ItemCode")),
                AttributesText = reader.IsDBNull(reader.GetOrdinal("AttributesText")) ? null : reader.GetString(reader.GetOrdinal("AttributesText")),
                AttributeValues = new List<ItemAttributeValue>()
            };
        }

        public async Task<List<ItemVariant>> GetByItemAsync(int itemId)
        {
            var list = new List<ItemVariant>();

            var query = $"{BaseSelect} WHERE v.ItemID = @ItemID ORDER BY v.Sort";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ItemID", itemId);

            using var reader = await cmd.ExecuteReaderAsync();

            var temp = new List<ItemVariant>();

            while (await reader.ReadAsync())
                temp.Add(Map(reader));

            reader.Close();

            foreach (var v in temp)
            {
                v.AttributeValues = await GetAttributesAsync(connection, v.ItemVariantID);
                list.Add(v);
            }

            return list;
        }

        private async Task<List<ItemAttributeValue>> GetAttributesAsync(SqlConnection connection, int variantId)
        {
            var list = new List<ItemAttributeValue>();

            var query = @"
                SELECT av.ItemAttributeValueID, av.ItemAttributeID,
                       av.ValueLabel, av.Abbreviation, a.AttributeName
                FROM ItemVariantAttributeValues vav
                INNER JOIN ItemAttributeValues av
                    ON av.ItemAttributeValueID = vav.ItemAttributeValueID
                INNER JOIN ItemAttributes a
                    ON a.ItemAttributeID = av.ItemAttributeID
                WHERE vav.ItemVariantID = @VariantID";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@VariantID", variantId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ItemAttributeValue
                {
                    ItemAttributeValueID = reader.GetInt32(0),
                    ItemAttributeID = reader.GetInt32(1),
                    ValueLabel = reader.GetString(2),
                    Abbreviation = reader.GetString(3),
                    AttributeName = reader.GetString(4)
                });
            }

            return list;
        }

        public async Task<ItemVariant?> GetByIdAsync(int id)
        {
            var query = $"{BaseSelect} WHERE v.ItemVariantID = @Id";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var variant = Map(reader);
            reader.Close();

            variant.AttributeValues = await GetAttributesAsync(connection, variant.ItemVariantID);

            return variant;
        }

        public async Task<int> AddAsync(ItemVariant variant, string username)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                    INSERT INTO ItemVariants
                    (ItemID, VariantCode, VariantName, IsActive, Sort, CreatedBy, CreatedDate, AttributesText)
                    VALUES
                    (@ItemID, @VariantCode, @VariantName, @IsActive, @Sort, @CreatedBy, GETDATE(), @AttributesText);
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(query, connection, transaction);

                cmd.Parameters.AddWithValue("@ItemID", variant.ItemID);
                cmd.Parameters.AddWithValue("@VariantCode", variant.VariantCode);
                cmd.Parameters.AddWithValue("@VariantName", variant.VariantName);
                cmd.Parameters.AddWithValue("@IsActive", variant.IsActive);
                cmd.Parameters.AddWithValue("@Sort", variant.Sort);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)username ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttributesText", (object?)variant.AttributesText ?? DBNull.Value);

                var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                if (variant.AttributeValues?.Count > 0)
                {
                    foreach (var av in variant.AttributeValues)
                    {
                        var q = @"
                            INSERT INTO ItemVariantAttributeValues
                            (ItemVariantID, ItemAttributeValueID)
                            VALUES (@VariantID, @ValueID)";

                        using var c = new SqlCommand(q, connection, transaction);
                        c.Parameters.AddWithValue("@VariantID", id);
                        c.Parameters.AddWithValue("@ValueID", av.ItemAttributeValueID);

                        await c.ExecuteNonQueryAsync();
                    }
                }

                transaction.Commit();
                return id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(ItemVariant variant, string username)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE ItemVariants
                SET VariantCode=@VariantCode,
                    VariantName=@VariantName,
                    IsActive=@IsActive,
                    Sort=@Sort,
                    AttributesText=@AttributesText,
                    UpdatedBy=@UpdatedBy,
                    UpdatedDate=GETDATE()
                WHERE ItemVariantID=@Id";

            using var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", variant.ItemVariantID);
            cmd.Parameters.AddWithValue("@VariantCode", variant.VariantCode);
            cmd.Parameters.AddWithValue("@VariantName", variant.VariantName);
            cmd.Parameters.AddWithValue("@IsActive", variant.IsActive);
            cmd.Parameters.AddWithValue("@Sort", variant.Sort);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AttributesText", (object?)variant.AttributesText ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var cmd = new SqlCommand(
                    "DELETE FROM ItemVariantAttributeValues WHERE ItemVariantID = @Id",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new SqlCommand(
                    "DELETE FROM ItemVariants WHERE ItemVariantID = @Id",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteByItemAsync(int itemId)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var ids = new List<int>();

                using (var cmd = new SqlCommand(
                    "SELECT ItemVariantID FROM ItemVariants WHERE ItemID = @ItemID",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);

                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                        ids.Add(reader.GetInt32(0));
                }

                foreach (var id in ids)
                {
                    using var cmd = new SqlCommand(
                        "DELETE FROM ItemVariantAttributeValues WHERE ItemVariantID = @Id",
                        connection, transaction);

                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }

                using var cmd2 = new SqlCommand(
                    "DELETE FROM ItemVariants WHERE ItemID = @ItemID",
                    connection, transaction);

                cmd2.Parameters.AddWithValue("@ItemID", itemId);
                await cmd2.ExecuteNonQueryAsync();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int itemId, string variantCode, int? excludeId = null)
        {
            var query = @"
                SELECT COUNT(1)
                FROM ItemVariants
                WHERE ItemID = @ItemID AND VariantCode = @Code";

            if (excludeId.HasValue)
                query += " AND ItemVariantID <> @ExcludeId";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ItemID", itemId);
            cmd.Parameters.AddWithValue("@Code", variantCode);
            if (excludeId.HasValue)
                cmd.Parameters.AddWithValue("@ExcludeId", excludeId.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0) > 0;
        }

        public async Task SetActiveAsync(int variantId, bool isActive, string username)
        {
            var query = @"
                UPDATE ItemVariants
                SET IsActive = @IsActive,
                    UpdatedBy = @Username,
                    UpdatedDate = GETDATE()
                WHERE ItemVariantID = @Id";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Id", variantId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> HasStockAsync(int variantId)
        {
            var query = @"
                SELECT COUNT(1)
                FROM ItemStock
                WHERE ItemVariantID = @Id AND Quantity > 0";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", variantId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result ?? 0) > 0;
        }
    }
}