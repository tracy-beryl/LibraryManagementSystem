using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerResourceSummaryViewModel
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string UnitCode { get; set; }
        public string ResourceType { get; set; }
        public int UsageCount { get; set; }
    }
}
