using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class PastPaperZipImportViewModel
    {
        [Required]
        public IFormFile ZipFile { get; set; }

        [Required]
        public IFormFile ExcelFile { get; set; }
    }
}