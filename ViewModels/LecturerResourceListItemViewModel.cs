using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerResourceListItemViewModel
    {
        public int ResourceId { get; set; }
        public int CompetencyStandardId { get; set; }

        public string ReferenceNumber { get; set; }
        public string Title { get; set; }
        public string ResourceType { get; set; }

        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Program { get; set; }
        public string Level { get; set; }
        public int Semester { get; set; }

        public bool IsActive { get; set; }
        public string UrlOrFilePath { get; set; }
        public string CreatedByUserId { get; set; }



    }
}
