using ItemManager.Core.Models;

namespace ItemManager.Web.ViewModels
{
    public class ItemViewModel
    {
        public int ItemID { get; set; } = 0;
        public string ItemName { get; set; } = string.Empty;
        public int ItemTypeID { get; set; }
        public int Sort { get; set; }
        // Dropdown
        public List<ItemType> ItemTypes { get; set; } = new List<ItemType>();
    }
}
