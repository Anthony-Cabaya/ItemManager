using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemAttributeValueViewModel
    {
        public int ItemAttributeValueID { get; set; }

        public int ItemAttributeID { get; set; }

        [Required(ErrorMessage = "Value Label is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string ValueLabel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Abbreviation is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed.")]
        public string Abbreviation { get; set; } = string.Empty;

        public int Sort { get; set; }

        public string? AttributeName { get; set; }
    }
}