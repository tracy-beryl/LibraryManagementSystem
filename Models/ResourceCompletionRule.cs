using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class ResourceCompletionRule
    {
        public int ResourceId { get; set; }
        public int RequiredWatchPercentage { get; set; }  // for videos
        public int RequiredQuizScore { get; set; }        // for quizzes
    }
}
