using System.ComponentModel.DataAnnotations;

namespace ItemManager.Core.Models
{
    public class Item
    {
        public int ItemID { get; set; }

        [Required, StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ItemTypeID { get; set; }

        public ItemType? ItemType { get; set; }

        [Range(0, 9999)]
        public int Sort { get; set; }

        [StringLength(200)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        [StringLength(200)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? ItemSubTypeID { get; set; }

        public string? ItemSubTypeName { get; set; }

        public string? ItemCode { get; set; }
        public string? Condition { get; set; }

        // Unit fields
        public int? BaseUnitID { get; set; }
        public int? DisplayUnitID { get; set; }

        public string? BaseUnitAbbreviation { get; set; }
        public string? DisplayUnitAbbreviation { get; set; }

    }
}
