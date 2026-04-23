using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class UnitCategoryViewModel
    {
        public int UnitCategoryID { get; set; } = 0;

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters allowed.")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 9999, ErrorMessage = "Sort must be between 0 and 9999.")]
        [Display(Name = "Sort Order")]
        public int Sort { get; set; }

        // Audit Fields (Index/Admin only)
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Updated By")]
        public string? UpdatedBy { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        public bool IsSystem { get; set; }

    }
}
