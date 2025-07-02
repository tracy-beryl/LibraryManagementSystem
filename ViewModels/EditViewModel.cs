using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class EditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string Category { get; set; }
        public string BorrowingDays { get; set; }

        public string PhotoPath { get; set; }

    }
}
