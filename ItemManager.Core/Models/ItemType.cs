using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemManager.Core.Models
{
    public class ItemType
    {
        public int ItemTypeID { get; set; }
        public string ItemTypeName { get; set; } = string.Empty;
        public int Sort { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
    }
}
