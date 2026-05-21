namespace ItemManager.Core.Models
{
    public class ItemAttribute
    {
        public int ItemAttributeID { get; set; }

        public int ItemID { get; set; }

        public string AttributeName { get; set; } = string.Empty;

        public int Sort { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        // Display only

        public List<ItemAttributeValue> Values { get; set; } = new();
    }
}