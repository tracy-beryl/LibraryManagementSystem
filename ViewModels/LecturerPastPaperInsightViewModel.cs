using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerPastPaperInsightViewModel
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string UnitCode { get; set; }
        public int AttemptCount { get; set; }
        public double AverageDifficulty { get; set; }
        public double AverageConfidence { get; set; }
    }
}
