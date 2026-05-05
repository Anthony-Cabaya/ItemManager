using ItemManager.Core.Models;

namespace ItemManager.Web.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new DashboardStats();

        public string ItemsWithoutUnitCssClass =>
            Stats.ItemsWithoutUnit == 0 ? "text-success" : "text-warning";

        public string ItemsWithoutSubTypeCssClass =>
            Stats.ItemsWithoutSubType == 0 ? "text-success" : "text-warning";

        public int TotalItems => Stats.TotalItems;
        public int ItemsThisMonth => Stats.ItemsThisMonth;
        public int ItemsWithoutUnit => Stats.ItemsWithoutUnit;
        public int ItemsWithoutSubType => Stats.ItemsWithoutSubType;
        public int TotalUnits => Stats.TotalUnits;
        public int TotalUnitCategories => Stats.TotalUnitCategories;
    }
}