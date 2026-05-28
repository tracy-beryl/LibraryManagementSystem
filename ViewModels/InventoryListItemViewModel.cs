using System;

namespace LibraryManagementSystem.ViewModels
{
    public class InventoryListItemViewModel
    {
        public int InventoryRecordId { get; set; }
        public int BookId { get; set; }

        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int DamagedCopies { get; set; }
        public int MissingCopies { get; set; }
        public int ReorderThreshold { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public bool IsLowStock => AvailableCopies <= ReorderThreshold;
        public bool HasDamage => DamagedCopies > 0;
        public bool HasMissing => MissingCopies > 0;
    }
}