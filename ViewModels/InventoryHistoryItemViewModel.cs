using System;

namespace LibraryManagementSystem.ViewModels
{
    public class InventoryHistoryItemViewModel
    {
        public string TransactionType { get; set; }
        public int QuantityChanged { get; set; }
        public int PreviousAvailableCopies { get; set; }
        public int NewAvailableCopies { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PerformedByName { get; set; }
    }
}