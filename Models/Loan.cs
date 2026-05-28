using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime LoanDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now;

        public DateTime? ReturnDate { get; set; }

        public LoanStatus Status { get; set; } = LoanStatus.Borrowed;

        public decimal? FrozenFineAmount { get; set; }
        public decimal? ReplacementCost { get; set; }

        public FinePaymentStatus FinePaymentStatus { get; set; } = FinePaymentStatus.None;
        public decimal FineAmountPaid { get; set; } = 0;
        public DateTime? FinePaidOn { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        public bool MpesaPaymentPending { get; set; } = false;
        public string CheckoutRequestId { get; set; }
        public string MpesaReceiptNumber { get; set; }
        public string TransactionReference { get; set; }
    }
}