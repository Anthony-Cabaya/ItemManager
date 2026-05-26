namespace ItemManager.Core.Models
{
    public class ItemVariant
    {
        public int ItemVariantID { get; set; }

        public int ItemID { get; set; }

        public string VariantCode { get; set; } = string.Empty;

        public string VariantName { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int Sort { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Display only

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public List<ItemAttributeValue> AttributeValues { get; set; } = new();

        public string? AttributesText { get; set; }

        public decimal Quantity { get; set; }

        public decimal ReservedQuantity { get; set; }

        public decimal AvailableQuantity => Quantity - ReservedQuantity;
    }
}