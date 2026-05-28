using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly LibraryDbContext _context;

        public RecommendationService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<List<RecommendedBookItemViewModel>> GetRecommendedBooksForUserAsync(string userId, int count = 6)
        {
            var borrowedBookIds = await _context.Loans
                .Where(l => l.UserId == userId)
                .Select(l => l.BookId)
                .Distinct()
                .ToListAsync();

            var reservedBookIds = await _context.BookReservations
                .Where(r => r.UserId == userId && !r.IsFulfilled)
                .Select(r => r.BookId)
                .Distinct()
                .ToListAsync();

            var interactedBookIds = borrowedBookIds
                .Union(reservedBookIds)
                .Distinct()
                .ToList();

            var recommendations = new List<RecommendedBookItemViewModel>();

            // -------------------------------
            // 1. Build category preferences
            // -------------------------------
            var categoryScores = new Dictionary<string, int>();
            var departmentScores = new Dictionary<string, int>();

            var loanBooks = await _context.Loans
                .Where(l => l.UserId == userId)
                .Include(l => l.Book)
                .Select(l => l.Book)
                .Where(b => b != null)
                .ToListAsync();

            foreach (var book in loanBooks)
            {
                if (!string.IsNullOrWhiteSpace(book.Category))
                {
                    if (!categoryScores.ContainsKey(book.Category))
                        categoryScores[book.Category] = 0;

                    categoryScores[book.Category] += 2;
                }

                if (!string.IsNullOrWhiteSpace(book.Department))
                {
                    if (!departmentScores.ContainsKey(book.Department))
                        departmentScores[book.Department] = 0;

                    departmentScores[book.Department] += 2;
                }
            }

            var reservedBooks = await _context.BookReservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .Select(r => r.Book)
                .Where(b => b != null)
                .ToListAsync();

            foreach (var book in reservedBooks)
            {
                if (!string.IsNullOrWhiteSpace(book.Category))
                {
                    if (!categoryScores.ContainsKey(book.Category))
                        categoryScores[book.Category] = 0;

                    categoryScores[book.Category] += 1;
                }

                if (!string.IsNullOrWhiteSpace(book.Department))
                {
                    if (!departmentScores.ContainsKey(book.Department))
                        departmentScores[book.Department] = 0;

                    departmentScores[book.Department] += 1;
                }
            }

            var topCategories = categoryScores
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Take(3)
                .ToList();

            var topDepartments = departmentScores
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Take(3)
                .ToList();

            // --------------------------------
            // 2. Category-based recommendations
            // --------------------------------
            if (topCategories.Any())
            {
                var categoryMatches = await _context.Books
                    .Include(b => b.Loans)
                    .Where(b =>
                        b.IsBorrowable &&
                        b.AvailableCopies > 0 &&
                        !interactedBookIds.Contains(b.Id) &&
                        !string.IsNullOrWhiteSpace(b.Category) &&
                        topCategories.Contains(b.Category))
                    .OrderByDescending(b => b.Loans.Count)
                    .Take(count)
                    .Select(b => new RecommendedBookItemViewModel
                    {
                        BookId = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        PhotoPath = b.PhotoPath,
                        Category = b.Category,
                        Department = b.Department,
                        AvailableCopies = b.AvailableCopies,
                        Reason = "Based on your borrowing and reservation categories"
                    })
                    .ToListAsync();

                recommendations.AddRange(categoryMatches);
            }

            // ----------------------------------
            // 3. Department-based recommendations
            // ----------------------------------
            if (recommendations.Count < count && topDepartments.Any())
            {
                var existingIds = recommendations.Select(r => r.BookId).ToList();

                var departmentMatches = await _context.Books
                    .Include(b => b.Loans)
                    .Where(b =>
                        b.IsBorrowable &&
                        b.AvailableCopies > 0 &&
                        !interactedBookIds.Contains(b.Id) &&
                        !existingIds.Contains(b.Id) &&
                        !string.IsNullOrWhiteSpace(b.Department) &&
                        topDepartments.Contains(b.Department))
                    .OrderByDescending(b => b.Loans.Count)
                    .Take(count - recommendations.Count)
                    .Select(b => new RecommendedBookItemViewModel
                    {
                        BookId = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        PhotoPath = b.PhotoPath,
                        Category = b.Category,
                        Department = b.Department,
                        AvailableCopies = b.AvailableCopies,
                        Reason = "Related to your preferred department"
                    })
                    .ToListAsync();

                recommendations.AddRange(departmentMatches);
            }

            // -----------------------------------
            // 4. Borrowed-together recommendations
            // -----------------------------------
            if (recommendations.Count < count && interactedBookIds.Any())
            {
                var existingIds = recommendations.Select(r => r.BookId).ToList();

                // Find other users who borrowed the same books as this user
                var similarUserIds = await _context.Loans
                    .Where(l => interactedBookIds.Contains(l.BookId) && l.UserId != userId)
                    .GroupBy(l => l.UserId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(20)
                    .ToListAsync();

                if (similarUserIds.Any())
                {
                    var borrowedTogetherIds = await _context.Loans
                        .Where(l =>
                            similarUserIds.Contains(l.UserId) &&
                            !interactedBookIds.Contains(l.BookId))
                        .GroupBy(l => l.BookId)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .Take(count * 2)
                        .ToListAsync();

                    var borrowedTogetherBooks = await _context.Books
                        .Include(b => b.Loans)
                        .Where(b =>
                            borrowedTogetherIds.Contains(b.Id) &&
                            b.IsBorrowable &&
                            b.AvailableCopies > 0 &&
                            !existingIds.Contains(b.Id))
                        .OrderByDescending(b => b.Loans.Count)
                        .Take(count - recommendations.Count)
                        .Select(b => new RecommendedBookItemViewModel
                        {
                            BookId = b.Id,
                            Title = b.Title,
                            Author = b.Author,
                            PhotoPath = b.PhotoPath,
                            Category = b.Category,
                            Department = b.Department,
                            AvailableCopies = b.AvailableCopies,
                            Reason = "Students with similar interests also borrowed this"
                        })
                        .ToListAsync();

                    recommendations.AddRange(borrowedTogetherBooks);
                }
            }

            // ------------------------
            // 5. Popularity as fallback
            // ------------------------
            if (recommendations.Count < count)
            {
                var existingIds = recommendations.Select(r => r.BookId).ToList();

                var fallbackBooks = await _context.Books
                    .Include(b => b.Loans)
                    .Where(b =>
                        b.IsBorrowable &&
                        b.AvailableCopies > 0 &&
                        !interactedBookIds.Contains(b.Id) &&
                        !existingIds.Contains(b.Id))
                    .OrderByDescending(b => b.Loans.Count)
                    .Take(count - recommendations.Count)
                    .Select(b => new RecommendedBookItemViewModel
                    {
                        BookId = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        PhotoPath = b.PhotoPath,
                        Category = b.Category,
                        Department = b.Department,
                        AvailableCopies = b.AvailableCopies,
                        Reason = "Popular with other students"
                    })
                    .ToListAsync();

                recommendations.AddRange(fallbackBooks);
            }

            return recommendations
                .GroupBy(r => r.BookId)
                .Select(g => g.First())
                .Take(count)
                .ToList();
        }
    }
}