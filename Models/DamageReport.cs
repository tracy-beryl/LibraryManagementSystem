using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class DamageReport
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public string ReportType { get; set; } // Damaged or Missing

        [Required]
        public string Description { get; set; }

        public int QuantityAffected { get; set; } = 1;

        public string Status { get; set; } = "Open"; // Open, Resolved, WrittenOff

        public DateTime ReportedAt { get; set; } = DateTime.Now;

        public string ReportedByUserId { get; set; }
        public ApplicationUser ReportedByUser { get; set; }
    }
}