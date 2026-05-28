using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string ISBN { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string Category { get; set; }
        [Required]
        public string BorrowingDays { get; set; }
        [Required]
        public string ShelfNumber { get; set; }
        public string Department { get; set; }
        public string Edition { get; set; }
        public string PhotoPath { get; set; }
        public ICollection<Loan> Loans { get; set; }
        public bool IsBorrowable { get; set; }
        public string ReferenceNumber { get; set; }
        public InventoryRecord InventoryRecord { get; set; }
        public ICollection<DamageReport> DamageReports { get; set; }

    }
}
