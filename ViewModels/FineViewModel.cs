using LibraryManagementSystem.Models;
using System;

namespace LibraryManagementSystem.ViewModels
{
    public class FineViewModel
    {
        public int LoanId { get; set; }

        public string StudentName { get; set; }
        public string BookTitle { get; set; }

        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int DaysLate { get; set; }
        public decimal FineAmount { get; set; }

        public LoanStatus Status { get; set; }
        public FinePaymentStatus FinePaymentStatus { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public bool IsPaid => FinePaymentStatus == FinePaymentStatus.Paid;
        public bool IsPending => FinePaymentStatus == FinePaymentStatus.Pending;
        public bool IsReturned => ReturnDate.HasValue;
    }
}