using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemAttributeRepository : IItemAttributeRepository
    {
        private readonly DbHelper _dbHelper;

        public ItemAttributeRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private const string BaseSelect = @"
            SELECT a.ItemAttributeID, a.ItemID, a.AttributeName, a.Sort,
                   a.CreatedBy, a.CreatedDate, a.UpdatedBy, a.UpdatedDate
            FROM ItemAttributes a";

        private static ItemAttribute Map(SqlDataReader reader)
        {
            return new ItemAttribute
            {
                ItemAttributeID = reader.GetInt32(reader.GetOrdinal("ItemAttributeID")),
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                AttributeName = reader.GetString(reader.GetOrdinal("AttributeName")),
                Sort = reader.GetInt32(reader.GetOrdinal("Sort")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy"))
                    ? null : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedDate")),
                Values = new List<ItemAttributeValue>()
            };
        }

        private async Task<List<ItemAttributeValue>> GetValuesAsync(SqlConnection connection, int attributeId)
        {
            var list = new List<ItemAttributeValue>();

            var query = @"
                SELECT av.ItemAttributeValueID, av.ItemAttributeID,
                       av.ValueLabel, av.Abbreviation, a.AttributeName, av.Sort
                FROM ItemAttributeValues av
                INNER JOIN ItemAttributes a ON a.ItemAttributeID = av.ItemAttributeID
                WHERE av.ItemAttributeID = @ItemAttributeID
                ORDER BY av.Sort";

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ItemAttributeID", attributeId);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new ItemAttributeValue
                {
                    ItemAttributeValueID = reader.GetInt32(reader.GetOrdinal("ItemAttributeValueID")),
                    ItemAttributeID = reader.GetInt32(reader.GetOrdinal("ItemAttributeID")),
                    ValueLabel = reader.GetString(reader.GetOrdinal("ValueLabel")),
                    Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation")),
                    Sort = reader.GetInt32(reader.GetOrdinal("Sort")),
                    AttributeName = reader.IsDBNull(reader.GetOrdinal("AttributeName"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("AttributeName"))
                });
            }

            return list;
        }

        public async Task<List<ItemAttribute>> GetByItemAsync(int itemId)
        {
            var list = new List<ItemAttribute>();

            var query = $"{BaseSelect} WHERE a.ItemID = @ItemID ORDER BY a.Sort";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@ItemID", itemId);

            using var reader = await cmd.ExecuteReaderAsync();

            var temp = new List<ItemAttribute>();

            while (await reader.ReadAsync())
            {
                temp.Add(Map(reader));
            }

            reader.Close();

            foreach (var attr in temp)
            {
                attr.Values = await GetValuesAsync(connection, attr.ItemAttributeID);
                list.Add(attr);
            }

            return list;
        }

        public async Task<ItemAttribute?> GetByIdAsync(int id)
        {
            var query = $"{BaseSelect} WHERE a.ItemAttributeID = @Id";

            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            var attribute = Map(reader);
            reader.Close();

            attribute.Values = await GetValuesAsync(connection, attribute.ItemAttributeID);

            return attribute;
        }

        public async Task<int> AddAsync(ItemAttribute attribute, string username)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var query = @"
                    INSERT INTO ItemAttributes
                    (ItemID, AttributeName, Sort, CreatedBy, CreatedDate)
                    VALUES
                    (@ItemID, @AttributeName, @Sort, @CreatedBy, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                using var cmd = new SqlCommand(query, connection, transaction);

                cmd.Parameters.AddWithValue("@ItemID", attribute.ItemID);
                cmd.Parameters.AddWithValue("@AttributeName", attribute.AttributeName);
                cmd.Parameters.AddWithValue("@Sort", attribute.Sort);
                cmd.Parameters.AddWithValue("@CreatedBy", (object?)username ?? DBNull.Value);

                var id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                if (attribute.Values != null && attribute.Values.Count > 0)
                {
                    foreach (var v in attribute.Values)
                    {
                        var vQuery = @"
                            INSERT INTO ItemAttributeValues
                            (ItemAttributeID, ValueLabel, Abbreviation, Sort, CreatedBy, CreatedDate)
                            VALUES
                            (@ItemAttributeID, @ValueLabel, @Abbreviation, @Sort, @CreatedBy, GETDATE())";

                        using var vCmd = new SqlCommand(vQuery, connection, transaction);

                        vCmd.Parameters.AddWithValue("@ItemAttributeID", id);
                        vCmd.Parameters.AddWithValue("@ValueLabel", v.ValueLabel);
                        vCmd.Parameters.AddWithValue("@Abbreviation", v.Abbreviation);
                        vCmd.Parameters.AddWithValue("@Sort", v.Sort);
                        vCmd.Parameters.AddWithValue("@CreatedBy", (object?)username ?? DBNull.Value);

                        await vCmd.ExecuteNonQueryAsync();
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

        public async Task UpdateAsync(ItemAttribute attribute, string username)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            var query = @"
                UPDATE ItemAttributes
                SET AttributeName = @AttributeName,
                    Sort = @Sort,
                    UpdatedBy = @UpdatedBy,
                    UpdatedDate = GETDATE()
                WHERE ItemAttributeID = @Id";

            using var cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", attribute.ItemAttributeID);
            cmd.Parameters.AddWithValue("@AttributeName", attribute.AttributeName);
            cmd.Parameters.AddWithValue("@Sort", attribute.Sort);
            cmd.Parameters.AddWithValue("@UpdatedBy", (object?)username ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _dbHelper.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var cmd1 = new SqlCommand(
                    "DELETE FROM ItemAttributeValues WHERE ItemAttributeID = @Id",
                    connection, transaction))
                {
                    cmd1.Parameters.AddWithValue("@Id", id);
                    await cmd1.ExecuteNonQueryAsync();
                }

                using (var cmd2 = new SqlCommand(
                    "DELETE FROM ItemAttributes WHERE ItemAttributeID = @Id",
                    connection, transaction))
                {
                    cmd2.Parameters.AddWithValue("@Id", id);
                    await cmd2.ExecuteNonQueryAsync();
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
                var attrIds = new List<int>();

                using (var cmd = new SqlCommand(
                    "SELECT ItemAttributeID FROM ItemAttributes " +
                    "WHERE ItemID = @ItemID",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                        attrIds.Add(reader.GetInt32(0));
                }

                if (attrIds.Count > 0)
                {
                    var valueIds = new List<int>();

                    var inClause = string.Join(",",
                        attrIds.Select((_, i) => $"@Aid{i}"));

                    var valueQuery =
                        $"SELECT ItemAttributeValueID " +
                        $"FROM ItemAttributeValues " +
                        $"WHERE ItemAttributeID IN ({inClause})";

                    using (var cmd = new SqlCommand(
                        valueQuery, connection, transaction))
                    {
                        for (int i = 0; i < attrIds.Count; i++)
                            cmd.Parameters.AddWithValue(
                                $"@Aid{i}", attrIds[i]);

                        using var reader =
                            await cmd.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                            valueIds.Add(reader.GetInt32(0));
                    }

                    if (valueIds.Count > 0)
                    {
                        var vInClause = string.Join(",",
                            valueIds.Select((_, i) => $"@Vid{i}"));

                        using var cmd = new SqlCommand(
                            $"DELETE FROM ItemVariantAttributeValues " +
                            $"WHERE ItemAttributeValueID IN ({vInClause})",
                            connection, transaction);

                        for (int i = 0; i < valueIds.Count; i++)
                            cmd.Parameters.AddWithValue(
                                $"@Vid{i}", valueIds[i]);

                        await cmd.ExecuteNonQueryAsync();
                    }

                    var avInClause = string.Join(",",
                        attrIds.Select((_, i) => $"@Aid{i}"));

                    using (var cmd = new SqlCommand(
                        $"DELETE FROM ItemAttributeValues " +
                        $"WHERE ItemAttributeID IN ({avInClause})",
                        connection, transaction))
                    {
                        for (int i = 0; i < attrIds.Count; i++)
                            cmd.Parameters.AddWithValue(
                                $"@Aid{i}", attrIds[i]);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                using (var cmd = new SqlCommand(
                    "DELETE FROM ItemAttributes " +
                    "WHERE ItemID = @ItemID",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ItemID", itemId);
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

    }
}