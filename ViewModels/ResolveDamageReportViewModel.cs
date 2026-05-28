using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class ResolveDamageReportViewModel
    {
        [Required]
        public int ReportId { get; set; }

        public int BookId { get; set; }

        public string BookTitle { get; set; }

        public string ReportType { get; set; }

        public int QuantityAffected { get; set; }

        public string Description { get; set; }

        [Required]
        public string ResolutionAction { get; set; } // Repaired, Recovered, WrittenOff

        [StringLength(500)]
        public string ResolutionNotes { get; set; }
    }
}