using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerDashboardViewModel
    {
        public string FirstName { get; set; }

        public int AssignedUnitsCount { get; set; }
        public int TotalResourcesCount { get; set; }
        public int PastPapersCount { get; set; }
        public int ActiveStudentsCount { get; set; }
        public int StudentsBehindCount { get; set; }

        public List<LecturerUnitSummaryViewModel> Units { get; set; } = new List<LecturerUnitSummaryViewModel>();
        public List<LecturerResourceSummaryViewModel> PopularResources { get; set; } = new List<LecturerResourceSummaryViewModel>();
        public List<LecturerStudentAlertViewModel> StudentsBehind { get; set; } = new List<LecturerStudentAlertViewModel>();
        public List<LecturerPastPaperInsightViewModel> PastPaperInsights { get; set; } = new List<LecturerPastPaperInsightViewModel>();
        public List<LecturerResourceGapAlertViewModel> ResourceGapAlerts { get; set; } = new List<LecturerResourceGapAlertViewModel>();
    }
}