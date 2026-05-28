using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class PaymentHistoryViewModel
    {
        public int LoanId { get; set; }
        public string StudentName { get; set; }
        public string BookTitle { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidOn { get; set; }
    }

}
