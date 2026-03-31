using ItemManager.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemViewModel
    {
        public int ItemID { get; set; } = 0;

        [Required(ErrorMessage = "Item Name is required.")]
        [StringLength(200, ErrorMessage = "Maximum 200 characters allowed.")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an Item Type.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select an Item Type.")]
        [Display(Name = "Item Type")]
        public int ItemTypeID { get; set; }

        [Range(0, 9999, ErrorMessage = "Sort must be between 0 and 9999.")]
        [Display(Name = "Sort Order")]
        public int Sort { get; set; }

        // Dropdown
        public List<ItemType> ItemTypes { get; set; } = new List<ItemType>();
    }
}
