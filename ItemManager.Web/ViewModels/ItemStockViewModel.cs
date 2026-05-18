using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemStockViewModel
    {
        public int StockID { get; set; }

        public int ItemID { get; set; }

        public int LocationID { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal Quantity { get; set; }

        [Range(0, 999999)]
        public decimal? MinStock { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public string? LocationName { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}