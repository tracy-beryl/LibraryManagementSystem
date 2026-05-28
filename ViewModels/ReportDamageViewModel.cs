using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class ReportDamageViewModel
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 100)]
        public int QuantityAffected { get; set; }

        [Required]
        public string ReportType { get; set; } // "Damaged" or "Missing"

        [Required]
        public string Description { get; set; }
    }
}