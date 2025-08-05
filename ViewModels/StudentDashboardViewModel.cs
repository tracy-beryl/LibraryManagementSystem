using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string FirstName { get; set; }
        public int MyBorrowedBooks { get; set; }
        public int MyHistory { get; set; }

        public List<Book> Books { get; set; }
    }
}
