using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemManager.Web.ViewModels
{
    public class TransactionLogViewModel
    {
        public int TransactionID { get; set; }

        public int ItemID { get; set; }

        public int LocationID { get; set; }

        public int? ItemVariantID { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Range(0.0001, 999999)]
        public decimal Quantity { get; set; }

        [MaxLength(255)]
        public string? ReferenceNote { get; set; }

        public DateTime TransactionDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? ItemName { get; set; }

        public string? ItemCode { get; set; }

        public string? LocationName { get; set; }

        public List<SelectListItem>? LocationList { get; set; }
    }
}