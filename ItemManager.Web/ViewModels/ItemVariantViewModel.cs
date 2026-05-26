using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemVariantViewModel
    {
        public int ItemVariantID { get; set; }

        [Required(ErrorMessage = "Item is required.")]
        public int ItemID { get; set; }

        [Required(ErrorMessage = "Variant Code is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string VariantCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Variant Name is required.")]
        [StringLength(255, ErrorMessage = "Maximum 255 characters allowed.")]
        public string VariantName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int Sort { get; set; }

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public List<ItemAttributeValueViewModel> AttributeValues { get; set; } = new();

        public string? AttributesText { get; set; }

        public decimal Quantity { get; set; }

        public decimal ReservedQuantity { get; set; }

        public decimal AvailableQuantity => Quantity - ReservedQuantity;
    }
}