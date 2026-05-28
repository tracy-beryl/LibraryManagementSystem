using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class ApproveSuggestionViewModel
    {
        public int SuggestionId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string BorrowingDays { get; set; }

        [Required]
        public string ShelfNumber { get; set; }

        public int TotalCopies { get; set; } = 1;
        public int AvailableCopies { get; set; } = 1;

        public string Category { get; set; }
        public string Department { get; set; }
        public string Edition { get; set; }

        public bool IsBorrowable { get; set; } = true;
    }
}