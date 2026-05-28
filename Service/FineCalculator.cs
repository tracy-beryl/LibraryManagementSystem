using System;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public static class FineCalculator
    {
        public static decimal CalculateFine(Loan loan)
        {
            // Lost book uses replacement cost
            if (loan.Status == LoanStatus.Lost)
                return loan.ReplacementCost ?? 0;

            // Once returned, fine must stop growing
            if (loan.FrozenFineAmount.HasValue)
                return loan.FrozenFineAmount.Value;

            // Not yet overdue
            if (DateTime.Now.Date <= loan.DueDate.Date)
                return 0;

            // Still borrowed and overdue -> live running fine
            int daysLate = (DateTime.Now.Date - loan.DueDate.Date).Days;
            return daysLate * 10;
        }
    }
}