namespace ItemManager.Core.Models
{
    public class ItemCodeSequence
    {
        public int SequenceID { get; set; }

        public int ItemTypeID { get; set; }

        public int? ItemSubTypeID { get; set; }

        public int LastSequence { get; set; }
    }
}