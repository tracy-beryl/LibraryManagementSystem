using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class LibrarianDashboardViewModel
    {
        public string FirstName { get; set; }

        public int BooksIssuedToday { get; set; }
        public int BooksReturnedToday { get; set; }
        public int ActiveLoansCount { get; set; }
        public int OverdueLoansCount { get; set; }
        public int UnpaidFinesCount { get; set; }
        public decimal UnpaidFinesAmount { get; set; }

        public int LowStockItemsCount { get; set; }
        public int DamagedItemsCount { get; set; }
        public int MissingItemsCount { get; set; }
        public int OpenDamageReportsCount { get; set; }

        public List<Loan> OverdueLoans { get; set; } = new List<Loan>();
        public List<Loan> RecentReturns { get; set; } = new List<Loan>();
        public List<Book> TopBorrowedBooks { get; set; } = new List<Book>();

        public List<InventoryRecord> LowStockItems { get; set; } = new List<InventoryRecord>();
        public List<InventoryRecord> DamagedItems { get; set; } = new List<InventoryRecord>();
        public List<InventoryRecord> MissingItems { get; set; } = new List<InventoryRecord>();
        public List<DamageReport> OpenDamageReports { get; set; } = new List<DamageReport>();
        public List<InventoryTransaction> RecentInventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}