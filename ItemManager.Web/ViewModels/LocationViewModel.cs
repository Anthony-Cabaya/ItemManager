using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class LocationViewModel
    {
        public int LocationID { get; set; }

        [Required]
        [MaxLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [Range(0, 9999)]
        public int Sort { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}