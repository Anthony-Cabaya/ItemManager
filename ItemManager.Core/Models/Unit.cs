using System.ComponentModel.DataAnnotations;

namespace ItemManager.Core.Models
{
    public class Unit
    {
        public int UnitID { get; set; }

        public int UnitCategoryID { get; set; }

        [Required, StringLength(100)]
        public string UnitName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Abbreviation { get; set; } = string.Empty;

        public bool IsSystem { get; set; }

        [Range(0, 9999)]
        public int Sort { get; set; }

        public string? UnitCategoryName { get; set; }

        public UnitCategory? UnitCategory { get; set; }

        [StringLength(200)]
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        [StringLength(200)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
