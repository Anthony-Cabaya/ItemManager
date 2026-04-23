using System.ComponentModel.DataAnnotations;

namespace ItemManager.Core.Models
{
    public class ItemSubType
    {
        public int ItemSubTypeID { get; set; }

        public int ItemTypeID { get; set; }

        [Required, StringLength(200)]
        public string ItemSubTypeName { get; set; } = string.Empty;

        [Range(0, 9999)]
        public int Sort { get; set; }

        public ItemType? ItemType { get; set; }
        public string? ItemTypeName { get; set; }

        [StringLength(200)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        [StringLength(200)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
