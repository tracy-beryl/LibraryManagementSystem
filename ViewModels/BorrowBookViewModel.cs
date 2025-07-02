
using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class BorrowBookViewModel
    {
        public int BookId { get; set; }
        public int LoanId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string BookTitle { get; set; }
        public string UserId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }

        public List<Book> Books { get; set; }
        public List<ApplicationUser> Users { get; set; }


        public List<Loan> Loans { get; set; } 
        
    }
}
