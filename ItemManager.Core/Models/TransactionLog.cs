namespace ItemManager.Core.Models
{
    public class TransactionLog
    {
        public int TransactionID { get; set; }

        public int ItemID { get; set; }

        public int LocationID { get; set; }

        public string TransactionType { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public string? ReferenceNote { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        // Display / Non-Mapped Properties

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public string? LocationName { get; set; }
    }
}