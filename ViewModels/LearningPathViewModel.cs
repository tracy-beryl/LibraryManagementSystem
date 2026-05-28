using LibraryManagementSystem.Models.CDACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LearningPathViewModel
    {
        public string Program { get; set; }
        public string Level { get; set; }

        public List<UnitLearningPath> Units { get; set; }
        public HashSet<int> CompletedResourceIds { get; set; } = new HashSet<int>();
        public HashSet<int> StartedResourceIds { get; set; } = new HashSet<int>();
    }

    public class UnitLearningPath
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public List<LibraryResource> Books { get; set; }
        public List<LibraryResource> PastPapers { get; set; }
        public List<LibraryResource> Videos { get; set; }
        public List<LibraryResource> WebLinks { get; set; }
        public List<LibraryResource> Documents { get; set; }

        public int TotalResources { get; set; }
        public int CompletedResources { get; set; }
        public int Percentage =>
            TotalResources == 0 ? 0 :
            (int)((double)CompletedResources / TotalResources * 100);
    }
}
