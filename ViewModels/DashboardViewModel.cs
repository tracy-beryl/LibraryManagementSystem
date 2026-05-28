using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public string FirstName { get; set; }
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int OverdueBooks { get; set; }
        public int PastPapers { get; set; }
        public int TotalVisitors { get; set; }
        public int NewMembers { get; set; }

        public List<ApplicationUser> Users { get; set; }
        public List<Book> Books { get; set; }
        public List<Loan> Loans { get; set; }
        public List<Book> TopBooks { get; set; }

        public List<VisitorStat> VisitorStats { get; set; }
        public List<CategoryData> CategoryChartData { get; set; }
    }

    public class VisitorStat
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

   

    public class CategoryData
    {
        public string Category { get; set; }
        public int AvailableCopies { get; set; }
    }

}






