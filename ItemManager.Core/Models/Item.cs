using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemManager.Core.Models
{
    public class Item
    {
        public int ItemID { get; set; }

        [Required, StringLength(200)]
        public string ItemName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ItemTypeID { get; set; }

        public ItemType? ItemType { get; set; }

        [Range(0, 9999)]
        public int Sort { get; set; }

        [StringLength(200)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        [StringLength(200)]
        public string UpdatedBy { get; set; } = string.Empty;

        public DateTime UpdatedDate { get; set; }
    }
}
