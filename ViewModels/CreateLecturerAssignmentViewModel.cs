using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class CreateLecturerAssignmentViewModel
    {
        [Required]
        public string LecturerUserId { get; set; }

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
    }
}