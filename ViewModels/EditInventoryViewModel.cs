using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class EditInventoryViewModel
    {
        [Required]
        public int InventoryRecordId { get; set; }

        public int BookId { get; set; }

        public string BookTitle { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public int DamagedCopies { get; set; }

        public int MissingCopies { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Reorder threshold must be 0 or greater.")]
        [Display(Name = "Reorder Threshold")]
        public int ReorderThreshold { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}