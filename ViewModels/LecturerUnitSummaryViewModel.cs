using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class LecturerUnitSummaryViewModel
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string Program { get; set; }
        public string Level { get; set; }
        public int Semester { get; set; }

        public int BooksCount { get; set; }
        public int DocumentsCount { get; set; }
        public int PastPapersCount { get; set; }
        public int VideosCount { get; set; }
        public int WebLinksCount { get; set; }

        public int TotalResourcesCount { get; set; }
    }
}