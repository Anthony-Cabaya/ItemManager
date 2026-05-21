using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemVariantMatrixRowViewModel
    {
        public bool IsChecked { get; set; }

        [Required(ErrorMessage = "Variant Code is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string VariantCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Variant Name is required.")]
        [StringLength(255, ErrorMessage = "Maximum 255 characters allowed.")]
        public string VariantName { get; set; } = string.Empty;

        public List<int> AttributeValueIds { get; set; } = new();
    }
}