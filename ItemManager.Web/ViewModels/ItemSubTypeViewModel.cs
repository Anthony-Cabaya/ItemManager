using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.ViewModels
{
    public class ItemSubTypeViewModel
    {
        public int ItemSubTypeID { get; set; } = 0;

        [Required(ErrorMessage = "Sub Type Name is required.")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters allowed.")]
        [Display(Name = "Sub Type Name")]
        public string ItemSubTypeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item Type is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Item Type.")]
        [Display(Name = "Item Type")]
        public int ItemTypeID { get; set; }

        [Display(Name = "Item Type Name")]
        public string? ItemTypeName { get; set; } 

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

        // Dropdown for Create/Edit
        public List<SelectListItem>? ItemTypeList { get; set; }
    }
}