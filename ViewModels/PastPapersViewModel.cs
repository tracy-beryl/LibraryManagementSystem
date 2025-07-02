using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class PastPapersViewModel
    {
        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseTitle { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        [Required]
        public string Semester { get; set; }

        public IFormFile File { get; set; } 
    }
}

