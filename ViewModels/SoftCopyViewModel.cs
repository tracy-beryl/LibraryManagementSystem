using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class SoftCopyViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public bool IsPublic { get; set; } = true;
    }
}
