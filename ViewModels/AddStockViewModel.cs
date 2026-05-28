using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class AddStockViewModel
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity added must be at least 1.")]
        [Display(Name = "Quantity Added")]
        public int QuantityAdded { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}