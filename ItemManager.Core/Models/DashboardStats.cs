namespace ItemManager.Core.Models
{
    public class DashboardStats
    {
        public int TotalItems { get; set; }
        public int ItemsThisMonth { get; set; }
        public int ItemsWithoutUnit { get; set; }
        public int ItemsWithoutSubType { get; set; }
        public int TotalUnits { get; set; }
        public int TotalUnitCategories { get; set; }
    }
}