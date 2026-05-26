using ItemManager.Core.Helpers;
using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DbHelper _dbHelper;

        public TransactionRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        private const string BaseSelect = @"
            SELECT
                t.TransactionID,
                t.ItemID,
                t.LocationID,
                t.ItemVariantID,
                t.TransactionType,
                t.Quantity,
                t.ReferenceNote,
                t.TransactionDate,
                t.CreatedBy,
                t.CreatedDate,
                i.ItemName,
                i.ItemCode,
                l.LocationName,
                v.VariantName
            FROM TransactionLog t
            INNER JOIN Items i ON t.ItemID = i.ItemID
            INNER JOIN Locations l ON t.LocationID = l.LocationID
            LEFT JOIN ItemVariants v ON t.ItemVariantID = v.ItemVariantID";

        private static TransactionLog Map(SqlDataReader reader)
        {
            return new TransactionLog
            {
                TransactionID = reader.GetInt32(reader.GetOrdinal("TransactionID")),
                ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                LocationID = reader.GetInt32(reader.GetOrdinal("LocationID")),
                ItemVariantID = reader.IsDBNull(reader.GetOrdinal("ItemVariantID"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("ItemVariantID")),
                TransactionType = reader.GetString(reader.GetOrdinal("TransactionType")),
                Quantity = reader.GetDecimal(reader.GetOrdinal("Quantity")),

                ReferenceNote = reader.IsDBNull(reader.GetOrdinal("ReferenceNote"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ReferenceNote")),

                TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),

                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("CreatedBy")),

                CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate"))
                    ? null
                    : reader.GetDateTime(reader.GetOrdinal("CreatedDate")),

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

        public async Task AddAsync(TransactionLog transaction)
        {
            const string query = @"
                INSERT INTO TransactionLog
                (
                    ItemID,
                    LocationID,
                    ItemVariantID,
                    TransactionType,
                    Quantity,
                    ReferenceNote,
                    TransactionDate,
                    CreatedBy,
                    CreatedDate
                )
                VALUES
                (
                    @ItemID,
                    @LocationID,
                    @ItemVariantID,
                    @TransactionType,
                    @Quantity,
                    @ReferenceNote,
                    @TransactionDate,
                    @CreatedBy,
                    @CreatedDate
                )";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ItemID", transaction.ItemID);
                command.Parameters.AddWithValue("@LocationID", transaction.LocationID);
                command.Parameters.AddWithValue("@ItemVariantID", (object?)transaction.ItemVariantID ?? DBNull.Value);
                command.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                command.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                command.Parameters.AddWithValue("@ReferenceNote", (object?)transaction.ReferenceNote ?? DBNull.Value);
                command.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                command.Parameters.AddWithValue("@CreatedBy", (object?)transaction.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", (object?)transaction.CreatedDate ?? DBNull.Value);

                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while adding transaction log.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding transaction log.", ex);
            }
        }

        public async Task<IEnumerable<TransactionLog>> GetByItemAsync(int itemId)
        {
            var list = new List<TransactionLog>();

            var query = $@"
                {BaseSelect}
                WHERE t.ItemID = @ItemID
                ORDER BY t.TransactionDate DESC";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", itemId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while retrieving item transactions.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving item transactions.", ex);
            }
        }

        public async Task<IEnumerable<TransactionLog>> GetByLocationAsync(int locationId)
        {
            var list = new List<TransactionLog>();

            var query = $@"
                {BaseSelect}
                WHERE t.LocationID = @LocationID
                ORDER BY t.TransactionDate DESC";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@LocationID", locationId);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while retrieving location transactions.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving location transactions.", ex);
            }
        }

        public async Task<IEnumerable<TransactionLog>> GetRecentAsync(int count = 50)
        {
            var list = new List<TransactionLog>();

            var query = @"
                SELECT TOP (@Count)
                    t.TransactionID,
                    t.ItemID,
                    t.LocationID,
                    t.ItemVariantID,
                    t.TransactionType,
                    t.Quantity,
                    t.ReferenceNote,
                    t.TransactionDate,
                    t.CreatedBy,
                    t.CreatedDate,
                    i.ItemName,
                    i.ItemCode,
                    l.LocationName,
                    v.VariantName
                FROM TransactionLog t
                INNER JOIN Items i ON t.ItemID = i.ItemID
                INNER JOIN Locations l ON t.LocationID = l.LocationID
                LEFT JOIN ItemVariants v ON t.ItemVariantID = v.ItemVariantID
                ORDER BY t.TransactionDate DESC";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Count", count);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(Map(reader));
                }

                return list;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while retrieving recent transactions.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving recent transactions.", ex);
            }
        }

        public async Task<PagedResult<TransactionLog>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string search = "",
            string transactionType = "")
        {
            var items = new List<TransactionLog>();

            var offset = (pageNumber - 1) * pageSize;

            var whereClause = @"
                WHERE
                (
                    i.ItemName LIKE @Search
                    OR i.ItemCode LIKE @Search
                )";

            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                whereClause += " AND t.TransactionType = @TransactionType";
            }

            var query = $@"
                {BaseSelect}
                {whereClause}
                ORDER BY t.TransactionDate DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM TransactionLog t
                INNER JOIN Items i ON t.ItemID = i.ItemID
                INNER JOIN Locations l ON t.LocationID = l.LocationID
                {whereClause};";

            try
            {
                using var connection = _dbHelper.CreateConnection();
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Search", $"%{search}%");
                command.Parameters.AddWithValue("@Offset", offset);
                command.Parameters.AddWithValue("@PageSize", pageSize);

                if (!string.IsNullOrWhiteSpace(transactionType))
                {
                    command.Parameters.AddWithValue("@TransactionType", transactionType);
                }

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    items.Add(Map(reader));
                }

                await reader.NextResultAsync();

                var totalCount = 0;

                if (await reader.ReadAsync())
                {
                    totalCount = reader.GetInt32(0);
                }

                return new PagedResult<TransactionLog>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("Database error occurred while retrieving paged transactions.", sqlEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving paged transactions.", ex);
            }
        }
    }
}