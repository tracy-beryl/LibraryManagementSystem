using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class ReportPageViewModel
    {
        public string Filter { get; set; }
        public string ReportTitle { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int TotalBooks { get; set; }
        public int TotalRows { get; set; }
        public int OverdueCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalOutstandingFines { get; set; }

        public List<ReportRowViewModel> Rows { get; set; } = new List<ReportRowViewModel>();
    }
}