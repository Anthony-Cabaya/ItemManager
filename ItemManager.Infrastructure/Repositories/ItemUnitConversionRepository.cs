using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Repositories
{
    public class ItemUnitConversionRepository : IItemUnitConversionRepository
    {
        private readonly DbHelper _dbHelper;

        // Constructor
        public ItemUnitConversionRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<ItemUnitConversion>> GetByItemIdAsync(int itemId)
        {
            var list = new List<ItemUnitConversion>();

            try
            {
                using var conn = _dbHelper.CreateConnection();
                using var cmd = new SqlCommand(@"
                    SELECT c.ConversionID, c.ItemID, c.UnitID, c.Factor,
                           u.UnitName, u.Abbreviation,
                           c.CreatedBy, c.CreatedDate,
                           c.UpdatedBy, c.UpdatedDate
                    FROM ItemUnitConversions c
                    INNER JOIN Units u ON c.UnitID = u.UnitID
                    WHERE c.ItemID = @ItemID
                    ORDER BY c.Factor ASC", conn);

                cmd.Parameters.AddWithValue("@ItemID", itemId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new ItemUnitConversion
                    {
                        ConversionID = (int)reader["ConversionID"],
                        ItemID = (int)reader["ItemID"],
                        UnitID = (int)reader["UnitID"],
                        Factor = (decimal)reader["Factor"],
                        UnitName = reader["UnitName"].ToString(),
                        Abbreviation = reader["Abbreviation"].ToString(),
                        CreatedBy = reader["CreatedBy"].ToString(),
                        CreatedDate = reader["CreatedDate"] as DateTime?,
                        UpdatedBy = reader["UpdatedBy"].ToString(),
                        UpdatedDate = reader["UpdatedDate"] as DateTime?
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching conversions", ex);
            }

            return list;
        }

        public async Task<ItemUnitConversion?> GetByIdAsync(int conversionId)
        {
            try
            {
                using var conn = _dbHelper.CreateConnection();
                using var cmd = new SqlCommand(@"
                    SELECT ConversionID, ItemID, UnitID, Factor
                    FROM ItemUnitConversions
                    WHERE ConversionID = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", conversionId);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new ItemUnitConversion
                    {
                        ConversionID = (int)reader["ConversionID"],
                        ItemID = (int)reader["ItemID"],
                        UnitID = (int)reader["UnitID"],
                        Factor = (decimal)reader["Factor"]
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching conversion", ex);
            }

            return null;
        }

        public async Task AddAsync(ItemUnitConversion model)
        {
            try
            {
                using var conn = _dbHelper.CreateConnection();
                using var cmd = new SqlCommand(@"
                    INSERT INTO ItemUnitConversions
                    (ItemID, UnitID, Factor, CreatedBy, CreatedDate)
                    VALUES
                    (@ItemID, @UnitID, @Factor, @CreatedBy, @CreatedDate)", conn);

                cmd.Parameters.AddWithValue("@ItemID", model.ItemID);
                cmd.Parameters.AddWithValue("@UnitID", model.UnitID);
                cmd.Parameters.AddWithValue("@Factor", model.Factor);
                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedDate", model.CreatedDate ?? DateTime.Now);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding conversion", ex);
            }
        }

        public async Task DeleteAsync(int conversionId)
        {
            try
            {
                using var conn = _dbHelper.CreateConnection();
                using var cmd = new SqlCommand(@"
                    DELETE FROM ItemUnitConversions
                    WHERE ConversionID = @Id", conn);

                cmd.Parameters.AddWithValue("@Id", conversionId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting conversion", ex);
            }
        }
    }
}