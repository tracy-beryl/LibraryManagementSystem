using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


    

namespace LibraryManagementSystem.Models
    {
        public class PastPaperAttempt
        {
            public int Id { get; set; }

            public int ResourceId { get; set; }
            public int StudentProfileId { get; set; }

            public int DifficultyRating { get; set; }     // 1 to 5
            public int ConfidenceRating { get; set; }     // 1 to 5

            public string ChallengingQuestions { get; set; } // e.g. "Q2, Q4, Q7"
            public string FeedbackNotes { get; set; }

            public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

            public StudentProfile StudentProfile { get; set; }
        }
    
}
