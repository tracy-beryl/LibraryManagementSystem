using System;

namespace LibraryManagementSystem.ViewModels
{
    public class ReportRowViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string ReferenceNumber { get; set; }

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int TimesBorrowed { get; set; }

        public string BorrowerName { get; set; }
        public DateTime? LoanDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }

        public decimal? FrozenFineAmount { get; set; }
        public decimal FineAmountPaid { get; set; }
        public decimal OutstandingFine { get; set; }
        public string FinePaymentStatus { get; set; }

        public bool MpesaPaymentPending { get; set; }
        public decimal? ReplacementCost { get; set; }
    }
}