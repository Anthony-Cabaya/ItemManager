using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemTypeViewModel
    {
        public int ItemTypeID { get; set; } = 0;

        [Required (ErrorMessage = "Item Type Name is required.")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters allowed.")]
        [Display(Name = "Item Type Name")]
        public string ItemTypeName { get; set; } = string.Empty;

        [Range(0, 9999, ErrorMessage = "Sort must be between 0 and 9999.")]
        [Display(Name = "Sort Order")]
        public int Sort { get; set; }
    }
}
