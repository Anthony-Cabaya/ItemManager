using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class ItemAttributeViewModel
    {
        public int ItemAttributeID { get; set; }

        public int ItemID { get; set; }

        [Required(ErrorMessage = "Attribute Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum 100 characters allowed.")]
        public string AttributeName { get; set; } = string.Empty;

        public int Sort { get; set; }

        public List<ItemAttributeValueViewModel> Values { get; set; } = new();
    }
}