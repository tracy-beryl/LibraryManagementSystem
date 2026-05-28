using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class PastPaperImportViewModel
    {
        [Required]
        public string CourseCode { get; set; }

        [Required]
        public string CourseTitle { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public PastPaperCategory Category { get; set; }

       
    }
}