using ItemManager.Core.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [Display(Name = "Item Code")]
        public string? ItemCode { get; set; }

        [Display(Name = "Condition")]
        public string? Condition { get; set; }

        // Dropdown
        public List<SelectListItem> ItemTypeOptions { get; set; } = new();
        public List<SelectListItem> SubTypeOptions { get; set; } = new();

        [Display(Name = "Item Sub Type")]
        public int? ItemSubTypeID { get; set; }

        [Display(Name = "Base Unit")]
        public int? BaseUnitID { get; set; }

        [Display(Name = "Display Unit")]
        public int? DisplayUnitID { get; set; }

        public string? BaseUnitAbbreviation { get; set; }

        public string? DisplayUnitAbbreviation { get; set; }

        public List<SelectListItem> UnitList { get; set; } = new();

        public static List<SelectListItem> ConditionOptions =>
            new()
            {
                new SelectListItem("New", "New"),
                new SelectListItem(
                    "Opened - Never Used",
                    "Opened - Never Used"),
                new SelectListItem("Used", "Used"),
                new SelectListItem("Defective", "Defective"),
                new SelectListItem("Disposed", "Disposed"),
                new SelectListItem("Discontinued", "Discontinued")
            };

        public class DeleteItemsRequest
        {
            public List<int> Ids { get; set; } = new();
        }

    }
}
