using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LecturerUnitAssignment
    {
        public int Id { get; set; }

        [Required]
        public string LecturerUserId { get; set; }
        public ApplicationUser LecturerUser { get; set; }

        [Required]
        public string Program { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        public string UnitCode { get; set; }

        [Required]
        public string UnitName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}