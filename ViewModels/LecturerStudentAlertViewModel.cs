using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerStudentAlertViewModel
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int CompletedResources { get; set; }
        public int TotalResources { get; set; }
        public int CompletionPercentage { get; set; }
    }
}
