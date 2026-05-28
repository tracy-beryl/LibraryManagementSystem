using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class ResourceCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public ResourceType Type { get; set; }

        // Used for Document, Book, PastPaper
        public IFormFile UploadedFile { get; set; }

        // Used for WebLink / YouTube
        [Url]
        public string ExternalUrl { get; set; }

        public string ReferenceNumber { get; set; }

        public string Author { get; set; }

        public string ISBN { get; set; }

        public IFormFile CoverImage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}