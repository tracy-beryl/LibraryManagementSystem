using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace LibraryManagementSystem.ViewModels
    {
        public class PastPaperAttemptHistoryViewModel
        {
            public List<PastPaperAttemptHistoryItemViewModel> Attempts { get; set; } = new List<PastPaperAttemptHistoryItemViewModel>();
        }

        public class PastPaperAttemptHistoryItemViewModel
        {
            public string Title { get; set; }

            public int DifficultyRating { get; set; }

            public int ConfidenceRating { get; set; }

            public string ChallengingQuestions { get; set; }

            public string FeedbackNotes { get; set; }

            public DateTime AttemptedAt { get; set; }
        }
    }


