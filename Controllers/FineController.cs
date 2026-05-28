using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Daraja;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.IO.Image;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class FineController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DarajaService _daraja;

        public FineController(
            LibraryDbContext context,
            UserManager<ApplicationUser> userManager,
            DarajaService daraja)
        {
            _context = context;
            _userManager = userManager;
            _daraja = daraja;
        }

        // ADMIN / LIBRARIAN VIEW 
        // Shows:
        // 1. live overdue fines for books not yet returned
        // 2. frozen fines for returned books
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Index()
        {
            var today = DateTime.Now.Date;

            var fines = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l =>
                    (l.ReturnDate == null && l.DueDate < today) ||
                    (l.ReturnDate != null && (l.FrozenFineAmount ?? 0) > 0))
                .AsEnumerable()
                .Select(l => new FineViewModel
                {
                    LoanId = l.Id,
                    StudentName = l.User?.FullName,
                    BookTitle = l.Book?.Title,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    DaysLate = l.ReturnDate == null
                        ? Math.Max(0, (today - l.DueDate.Date).Days)
                        : Math.Max(0, (l.ReturnDate.Value.Date - l.DueDate.Date).Days),
                    FineAmount = l.ReturnDate == null
                        ? FineCalculator.CalculateFine(l) // live running fine
                        : (l.FrozenFineAmount ?? 0),      // frozen fine after return
                    Status = l.Status,
                    FinePaymentStatus = l.FinePaymentStatus,
                    PaymentMethod = l.PaymentMethod
                })
                .ToList();

            return View(fines);
        }

        // ================= STUDENT VIEW =================
        // Shows only returned books with frozen fines
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyFines()
        {
            var user = await _userManager.GetUserAsync(User);

            var fines = _context.Loans
                .Include(l => l.Book)
                .Where(l =>
                    l.UserId == user.Id &&
                    l.ReturnDate != null &&
                    (l.FrozenFineAmount ?? 0) > 0)
                .Select(l => new FineViewModel
                {
                    LoanId = l.Id,
                    BookTitle = l.Book.Title,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    DaysLate = EF.Functions.DateDiffDay(l.DueDate, l.ReturnDate.Value),
                    FineAmount = l.FrozenFineAmount ?? 0,
                    Status = l.Status,
                    FinePaymentStatus = l.FinePaymentStatus,
                    PaymentMethod = l.PaymentMethod
                })
                .ToList();

            return View(fines);
        }

        // ================= CASH PAYMENT =================
        // Librarian records fine after return
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayCash(int loanId)
        {
            var loan = await _context.Loans
                .Include(l => l.User)
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.Id == loanId);

            if (loan == null)
                return Json(new { success = false, message = "Loan not found." });

            if (loan.ReturnDate == null)
                return Json(new { success = false, message = "Book must be returned before fine payment." });

            if ((loan.FrozenFineAmount ?? 0) <= 0)
                return Json(new { success = false, message = "No fine due." });

            if (loan.FinePaymentStatus == FinePaymentStatus.Paid)
                return Json(new { success = false, message = "Fine already paid." });

            if (loan.FinePaymentStatus == FinePaymentStatus.Pending)
                return Json(new { success = false, message = "M-Pesa payment is already in progress." });

            var amount = loan.FrozenFineAmount ?? 0;

            loan.FinePaymentStatus = FinePaymentStatus.Paid;
            loan.PaymentMethod = PaymentMethod.Cash;
            loan.FinePaidOn = DateTime.Now;
            loan.FineAmountPaid = amount;
            loan.MpesaPaymentPending = false;
            loan.CheckoutRequestId = null;

            _context.PaymentReceipts.Add(new PaymentReceipt
            {
                LoanId = loan.Id,
                Amount = amount
            });

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cash payment recorded successfully." });
        }

        //  MPESA INITIATION 
        // Librarian initiates STK push to student's phone after return
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitiateMpesa(int loanId, string phoneNumber)
        {
            try
            {
                var loan = await _context.Loans
                    .Include(l => l.User)
                    .Include(l => l.Book)
                    .FirstOrDefaultAsync(l => l.Id == loanId);

                if (loan == null)
                    return Json(new { success = false, message = "Loan not found." });

                if (loan.ReturnDate == null)
                    return Json(new { success = false, message = "Book must be returned before fine payment." });

                if ((loan.FrozenFineAmount ?? 0) <= 0)
                    return Json(new { success = false, message = "No fine due." });

                if (loan.FinePaymentStatus == FinePaymentStatus.Paid)
                    return Json(new { success = false, message = "Fine already paid." });

                if (loan.FinePaymentStatus == FinePaymentStatus.Pending)
                    return Json(new { success = false, message = "M-Pesa payment is already in progress." });

                var numberToUse = !string.IsNullOrWhiteSpace(phoneNumber)
                    ? phoneNumber
                    : loan.User?.PhoneNumber;

                if (string.IsNullOrWhiteSpace(numberToUse))
                    return Json(new { success = false, message = "Phone number is required." });

                var amount = loan.FrozenFineAmount ?? 0;

                var checkoutId = await _daraja.InitiateStkPush(
                    numberToUse,
                    amount,
                    loan.Id.ToString()
                );

                if (string.IsNullOrWhiteSpace(checkoutId))
                    return Json(new { success = false, message = "Failed to initiate M-Pesa payment." });

                loan.CheckoutRequestId = checkoutId;
                loan.FinePaymentStatus = FinePaymentStatus.Pending;
                loan.PaymentMethod = PaymentMethod.Mpesa;
                loan.MpesaPaymentPending = true;

                // optional: save the entered number back to the student record
                if (loan.User != null && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    loan.User.PhoneNumber = phoneNumber.Trim();
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "STK push sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ================= DARAJA CALLBACK =================
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Callback([FromBody] DarajaCallbackDto dto)
        {
            if (dto?.Body?.stkCallback == null)
                return Ok();

            var callback = dto.Body.stkCallback;
            var checkoutId = callback.CheckoutRequestID;

            if (string.IsNullOrWhiteSpace(checkoutId))
                return Ok();

            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.CheckoutRequestId == checkoutId);

            if (loan == null)
                return Ok();

            // failed / cancelled transaction
            if (callback.ResultCode != 0)
            {
                loan.MpesaPaymentPending = false;

                // keep as unpaid since payment did not complete
                if (loan.FinePaymentStatus == FinePaymentStatus.Pending)
                    loan.FinePaymentStatus = FinePaymentStatus.Unpaid;

                await _context.SaveChangesAsync();
                return Ok();
            }

            // safety check
            if (loan.ReturnDate == null || (loan.FrozenFineAmount ?? 0) <= 0)
            {
                loan.MpesaPaymentPending = false;
                await _context.SaveChangesAsync();
                return Ok();
            }

            var metadata = callback.CallbackMetadata?.Item;

            string mpesaReceiptNumber = metadata?
                .FirstOrDefault(x => x.Name == "MpesaReceiptNumber")?.Value?.ToString();

            loan.FinePaymentStatus = FinePaymentStatus.Paid;
            loan.PaymentMethod = PaymentMethod.Mpesa;
            loan.FinePaidOn = DateTime.Now;
            loan.FineAmountPaid = loan.FrozenFineAmount ?? 0;
            loan.MpesaPaymentPending = false;
            loan.MpesaReceiptNumber = mpesaReceiptNumber;

            _context.PaymentReceipts.Add(new PaymentReceipt
            {
                LoanId = loan.Id,
                Amount = loan.FrozenFineAmount ?? 0,
                CheckoutRequestId = checkoutId
            });

            await _context.SaveChangesAsync();

            return Ok();
        }

        // ================= STUDENT PAYMENT HISTORY =================
        public async Task<IActionResult> MyPayments()
        {
            var user = await _userManager.GetUserAsync(User);

            var payments = _context.PaymentReceipts
                .Include(r => r.Loan)
                .ThenInclude(l => l.Book)
                .Where(r => r.Loan.UserId == user.Id)
                .Select(r => new PaymentHistoryViewModel
                {
                    LoanId = r.LoanId,
                    BookTitle = r.Loan.Book.Title,
                    Amount = r.Amount,
                    PaidOn = r.PaidOn
                })
                .ToList();

            return View(payments);
        }
        // ADMIN / LIBRARIAN PAYMENT HISTORY 
        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Payments()
        {
            var payments = _context.PaymentReceipts
                .Include(r => r.Loan)
                .ThenInclude(l => l.Book)
                .Include(r => r.Loan.User)
                .Select(r => new PaymentHistoryViewModel
                {
                    LoanId = r.LoanId,
                    StudentName = r.Loan.User.FullName,
                    BookTitle = r.Loan.Book.Title,
                    Amount = r.Amount,
                    PaidOn = r.PaidOn
                })
                .ToList();

            return View(payments);
        }

        // ================= RECEIPT =================
      


     public IActionResult Receipt(int loanId)
    {
        var receipt = _context.PaymentReceipts
            .Include(r => r.Loan)
            .ThenInclude(l => l.Book)
            .Include(r => r.Loan.User)
            .FirstOrDefault(r => r.LoanId == loanId);

        if (receipt == null)
            return NotFound();

        var settings = _context.Settings.FirstOrDefault();

        using var stream = new MemoryStream();

        using (var writer = new PdfWriter(stream))
        using (var pdf = new PdfDocument(writer))
        using (var document = new Document(pdf))
        {
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            if (settings != null && !string.IsNullOrWhiteSpace(settings.LogoPath))
            {
                var logoFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", settings.LogoPath);

                if (System.IO.File.Exists(logoFullPath))
                {
                    var imageData = ImageDataFactory.Create(logoFullPath);
                    var logo = new Image(imageData)
                        .ScaleToFit(80, 80);

                    document.Add(logo);
                }
            }

            document.Add(
                new Paragraph(settings?.Name ?? "Library Management System")
                    .SetFont(boldFont)
                    .SetFontSize(16)
            );

            if (!string.IsNullOrWhiteSpace(settings?.ShortCode))
            {
                document.Add(
                    new Paragraph($"Code: {settings.ShortCode}")
                        .SetFont(normalFont)
                        .SetFontSize(10)
                );
            }

            document.Add(
                new Paragraph("Library Payment Receipt")
                    .SetFont(boldFont)
                    .SetFontSize(14)
            );

            document.Add(
                new Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy HH:mm}")
                    .SetFont(normalFont)
                    .SetFontSize(10)
            );

            document.Add(new Paragraph(" "));

            var table = new Table(2).UseAllAvailableWidth();

            table.AddCell(new Cell().Add(new Paragraph("Student").SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph(receipt.Loan?.User?.FullName ?? "N/A").SetFont(normalFont)));

            table.AddCell(new Cell().Add(new Paragraph("Book").SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph(receipt.Loan?.Book?.Title ?? "N/A").SetFont(normalFont)));

            table.AddCell(new Cell().Add(new Paragraph("Amount Paid").SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph($"KES {receipt.Amount:N2}").SetFont(normalFont)));

            table.AddCell(new Cell().Add(new Paragraph("Paid On").SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph($"{receipt.PaidOn:dd MMM yyyy HH:mm}").SetFont(normalFont)));

            table.AddCell(new Cell().Add(new Paragraph("Loan ID").SetFont(boldFont)));
            table.AddCell(new Cell().Add(new Paragraph(receipt.LoanId.ToString()).SetFont(normalFont)));

            if (!string.IsNullOrWhiteSpace(receipt.CheckoutRequestId))
            {
                table.AddCell(new Cell().Add(new Paragraph("Transaction Ref").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph(receipt.CheckoutRequestId).SetFont(normalFont)));
            }

            document.Add(table);

            document.Add(new Paragraph(" "));

            document.Add(
                new Paragraph("Thank you.")
                    .SetFont(normalFont)
                    .SetFontSize(11)
            );
        }

        return File(stream.ToArray(), "application/pdf", $"Receipt_{loanId}.pdf");
    }
    //  REVERSE PAYMENT 
    // Admin only. Reverses the fine payment, but does not change loan status.
    [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReversePayment(int loanId)
        {
            var loan = await _context.Loans.FindAsync(loanId);

            if (loan == null)
                return Json(new { success = false, message = "Loan not found." });

            if (loan.FinePaymentStatus != FinePaymentStatus.Paid)
                return Json(new { success = false, message = "Only paid fines can be reversed." });

            loan.FinePaymentStatus = FinePaymentStatus.Reversed;
            loan.FineAmountPaid = 0;
            loan.FinePaidOn = null;
            loan.PaymentMethod = null;
            loan.MpesaPaymentPending = false;
            loan.CheckoutRequestId = null;
            loan.MpesaReceiptNumber = null;

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Payment reversed successfully." });
        }

       
    }
}