using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }

        [Required]
        public int InventoryRecordId { get; set; }
        public InventoryRecord InventoryRecord { get; set; }

        [Required]
        public InventoryTransactionType TransactionType { get; set; }

        public int QuantityChanged { get; set; }

        public int PreviousAvailableCopies { get; set; }

        public int NewAvailableCopies { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string PerformedByUserId { get; set; }
        public ApplicationUser PerformedByUser { get; set; }
    }
}