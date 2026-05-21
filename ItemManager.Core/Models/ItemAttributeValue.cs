namespace ItemManager.Core.Models
{
    public class ItemAttributeValue
    {
        public int ItemAttributeValueID { get; set; }

        public int ItemAttributeID { get; set; }

        public string ValueLabel { get; set; } = string.Empty;

        public string Abbreviation { get; set; } = string.Empty;

        public int Sort { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        // Display only

        public string? AttributeName { get; set; }
    }
}