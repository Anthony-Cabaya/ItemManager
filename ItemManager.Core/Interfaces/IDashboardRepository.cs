using ItemManager.Core.Models;

namespace ItemManager.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardStats> GetStatsAsync();
    }
}