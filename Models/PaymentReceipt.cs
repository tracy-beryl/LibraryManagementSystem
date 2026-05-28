using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class PaymentReceipt
    {
        public int Id { get; set; }

        public int LoanId { get; set; }
        public Loan Loan { get; set; }

        public decimal Amount { get; set; }

        public string CheckoutRequestId { get; set; }

        public DateTime PaidOn { get; set; } = DateTime.Now;

        public string Method { get; set; } = "M-Pesa";
    }
}

