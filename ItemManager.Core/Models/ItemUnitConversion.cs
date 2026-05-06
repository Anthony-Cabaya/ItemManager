namespace ItemManager.Core.Models
{
    public class ItemUnitConversion
    {
        public int ConversionID { get; set; }
        public int ItemID { get; set; }
        public int UnitID { get; set; }
        public decimal Factor { get; set; }

        public string? UnitName { get; set; }
        public string? Abbreviation { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}