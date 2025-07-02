using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class PastPapers
    {
        public int Id { get; set; }

        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseTitle { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public string UploadedBy { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public string FilePath { get; set; }
    }
}

