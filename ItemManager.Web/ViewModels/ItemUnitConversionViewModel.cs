using ItemManager.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemUnitConversionViewModel
    {
        public int ConversionID { get; set; }
        public int ItemID { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string? BaseUnitName { get; set; }
        public string? BaseUnitAbbreviation { get; set; }

        [Required]
        public int UnitID { get; set; }

        [Required]
        [Range(0.000001, double.MaxValue,
            ErrorMessage = "Factor must be greater than 0")]
        public decimal Factor { get; set; }

        public string? UnitName { get; set; }
        public string? Abbreviation { get; set; }

        public List<SelectListItem> AvailableUnits { get; set; } = new();
        public List<ItemUnitConversion> Conversions { get; set; } = new();
    }
}