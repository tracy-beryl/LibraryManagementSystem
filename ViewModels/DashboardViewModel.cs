using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {



        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        
        public int OverdueBooks { get; set; }
        public int PastPapersCount { get; set; }

        public string ActiveView { get; set; }


        public string SearchString { get; set; }
        public string SearchType { get; set; }


    }
}
