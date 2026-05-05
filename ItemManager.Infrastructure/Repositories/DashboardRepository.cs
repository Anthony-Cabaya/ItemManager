using ItemManager.Core.Interfaces;
using ItemManager.Core.Models;
using ItemManager.Infrastructure.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ItemManager.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DbHelper _dbHelper;

        public DashboardRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<DashboardStats> GetStatsAsync()
        {
            try
            {
                await using var conn = _dbHelper.CreateConnection();
                await conn.OpenAsync();

                await using var cmd = new SqlCommand("sp_GetDashboardStats", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new DashboardStats
                    {
                        TotalItems = reader.GetInt32(reader.GetOrdinal("TotalItems")),
                        ItemsThisMonth = reader.GetInt32(reader.GetOrdinal("ItemsThisMonth")),
                        ItemsWithoutUnit = reader.GetInt32(reader.GetOrdinal("ItemsWithoutUnit")),
                        ItemsWithoutSubType = reader.GetInt32(reader.GetOrdinal("ItemsWithoutSubType")),
                        TotalUnits = reader.GetInt32(reader.GetOrdinal("TotalUnits")),
                        TotalUnitCategories = reader.GetInt32(reader.GetOrdinal("TotalUnitCategories"))
                    };
                }

                return new DashboardStats();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching dashboard stats", ex);
            }
        }
    }
}