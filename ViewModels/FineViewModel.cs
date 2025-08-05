using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class FineViewModel
    {
  
        public string StudentName { get; set; }
        public string BookTitle { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int DaysLate { get; set; }
        public decimal FineAmount { get; set; }
    }

}
