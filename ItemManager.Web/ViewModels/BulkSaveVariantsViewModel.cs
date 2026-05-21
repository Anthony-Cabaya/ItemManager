using System.ComponentModel.DataAnnotations;

namespace ItemManager.Web.ViewModels
{
    public class BulkSaveVariantsViewModel
    {
        [Required(ErrorMessage = "Item is required.")]
        public int ItemID { get; set; }

        public List<ItemVariantMatrixRowViewModel> Rows { get; set; } = new();
    }
}