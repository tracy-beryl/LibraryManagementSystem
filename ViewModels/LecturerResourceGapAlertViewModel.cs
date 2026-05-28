using System;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerResourceGapAlertViewModel
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Program { get; set; }
        public string Level { get; set; }
        public int Semester { get; set; }

        public int CompetencyStandardId { get; set; }
        public int TotalResourcesCount { get; set; }

        public bool NoResources { get; set; }
        public bool NoBooks { get; set; }
        public bool NoDocuments { get; set; }
        public bool NoPastPapers { get; set; }
        public bool NoVideos { get; set; }
        public bool BelowMinimumResources { get; set; }

        public string Summary { get; set; }
    }
}