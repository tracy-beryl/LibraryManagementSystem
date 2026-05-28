using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class StudentDashboardViewModel
    {
        public string FirstName { get; set; }

        public int MyBorrowedBooks { get; set; }
        public int MyHistory { get; set; }
        public int DueSoonCount { get; set; }
        public int OverdueCount { get; set; }

        public int MyActiveProjects { get; set; }
        public int PendingInvitations { get; set; }
        public int UnreadProjectMessages { get; set; }

        public int LearningResourcesTotal { get; set; }
        public int LearningResourcesCompleted { get; set; }
        public double LearningCompletionPercent { get; set; }
        public List<RecommendedBookItemViewModel> RecommendedBooks { get; set; } = new List<RecommendedBookItemViewModel>();

        public List<StudentLoanItemViewModel> CurrentLoans { get; set; } = new List<StudentLoanItemViewModel>();
        public List<StudentLoanItemViewModel> DueSoonBooks { get; set; } = new List<StudentLoanItemViewModel>();
        public List<StudentLoanItemViewModel> OverdueBooks { get; set; } = new List<StudentLoanItemViewModel>();
        public List<TopBorrowedBookViewModel> TopBorrowedBooks { get; set; } = new List<TopBorrowedBookViewModel>();

        public List<StudentProjectItemViewModel> ActiveProjects { get; set; } = new List<StudentProjectItemViewModel>();
        public List<StudentProjectDeadlineViewModel> UpcomingDeadlines { get; set; } = new List<StudentProjectDeadlineViewModel>();

        public List<LearningUnitProgressItemViewModel> UnitProgress { get; set; } = new List<LearningUnitProgressItemViewModel>();
        public List<LearningContinueItemViewModel> ContinueLearning { get; set; } = new List<LearningContinueItemViewModel>();
    }
    public class RecommendedBookItemViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PhotoPath { get; set; }
        public string Category { get; set; }
        public string Department { get; set; }
        public int AvailableCopies { get; set; }
        public string Reason { get; set; }
    }

    public class StudentLoanItemViewModel
    {
        public int LoanId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PhotoPath { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysLeft { get; set; }
    }

    public class TopBorrowedBookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PhotoPath { get; set; }
        public int BorrowCount { get; set; }
        public string Category { get; set; }
    }

    public class StudentProjectItemViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }
        public int TeamMembers { get; set; }
        public int ResourceCount { get; set; }
        public int DeadlineCount { get; set; }
        public int UnreadMessages { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class StudentProjectDeadlineViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string DeadlineTitle { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysLeft { get; set; }
    }

    public class LearningUnitProgressItemViewModel
    {
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int TotalResources { get; set; }
        public int CompletedResources { get; set; }
    }

    public class LearningContinueItemViewModel
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        public string ResourceType { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
    }
}