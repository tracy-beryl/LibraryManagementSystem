using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class AttemptPastPaperViewModel
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        [Required]
        [Range(1, 5)]
        public int DifficultyRating { get; set; }
        public int ConfidenceRating { get; set; }
        public string ChallengingQuestions { get; set; }
        public string FeedbackNotes { get; set; }
    }
}
