using LibraryManagementSystem.Models.CDACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class StudentResourceProgress
    {
        public int Id { get; set; }

        public int StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; }

        public int ResourceId { get; set; }
        public LibraryResource Resource { get; set; }
        public double WatchSeconds { get; set; }           // Videos
        public double TimeSpentSeconds { get; set; }       // Docs / Links
        public double PageCoveragePercent { get; set; }    // PDFs
        public int QuizScore { get; set; }                 // Books / Links
        public int AttemptCount { get; set; }              //Past Papers
        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }
    }
}
