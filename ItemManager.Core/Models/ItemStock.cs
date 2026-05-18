namespace ItemManager.Core.Models
{
    public class ItemStock
    {
        public int StockID { get; set; }

        public int ItemID { get; set; }

        public int LocationID { get; set; }

        public decimal Quantity { get; set; }

        public decimal? MinStock { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Display / Non-Mapped Properties

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public string? LocationName { get; set; }
    }
}