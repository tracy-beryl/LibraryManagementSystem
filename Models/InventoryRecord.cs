using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class InventoryRecord
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public int DamagedCopies { get; set; }

        public int MissingCopies { get; set; }

        public int ReorderThreshold { get; set; } = 2;

        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        public string LastUpdatedByUserId { get; set; }
        public ApplicationUser LastUpdatedByUser { get; set; }
    }
}