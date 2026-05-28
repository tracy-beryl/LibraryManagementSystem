using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class BookSuggestion
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }

        public string SuggestedByUserId { get; set; }
        public ApplicationUser SuggestedBy { get; set; }

        public DateTime SuggestedOn { get; set; } = DateTime.Now;

        public DateTime SuggestedAt { get; set; }
    
    public bool Approved { get; set; }
    }

}
