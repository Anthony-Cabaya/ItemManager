namespace ItemManager.Core.Models
{
    public class UnitCategory
    {
        public int UnitCategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int Sort { get; set; }

        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
