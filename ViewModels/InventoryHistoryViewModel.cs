using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class InventoryHistoryViewModel
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int DamagedCopies { get; set; }
        public int MissingCopies { get; set; }
        public int ReorderThreshold { get; set; }

        public List<InventoryHistoryItemViewModel> Transactions { get; set; } = new List<InventoryHistoryItemViewModel>();
    }
}