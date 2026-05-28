using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerAddResourceViewModel
    {
        [Required]
        public int CompetencyStandardId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please select a resource type.")]
        public ResourceType? Type { get; set; }

        public string Author { get; set; }
        public string ISBN { get; set; }

        public string ExternalUrl { get; set; }

        public IFormFile UploadedFile { get; set; }
        public IFormFile CoverImage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}