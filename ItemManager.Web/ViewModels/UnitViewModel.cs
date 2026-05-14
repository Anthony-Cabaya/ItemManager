using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class UnitViewModel
    {
        public int UnitID { get; set; }

        [Required(ErrorMessage = "Unit Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        [Display(Name = "Unit Name")]
        public string UnitName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Abbreviation is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed.")]
        [Display(Name = "Abbreviation")]
        public string Abbreviation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unit Category is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category.")]
        [Display(Name = "Unit Category")]
        public int UnitCategoryID { get; set; }

        public string? UnitCategoryName { get; set; }

        [Range(0, 9999, ErrorMessage = "Sort must be between 0 and 9999.")]
        [Display(Name = "Sort Order")]
        public int Sort { get; set; }

        public bool IsSystem { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<SelectListItem>? UnitCategoryList { get; set; }

    }
}