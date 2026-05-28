using LibraryManagementSystem.Models;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels
{
    public class ProjectDetailsViewModel
    {
        public int Id { get; set; }

        public string ProjectName { get; set; }

        public string Description { get; set; }

        public bool IsOwner { get; set; }

        public List<ProjectMember> Members { get; set; }

        public List<ResourceViewModel> Resources { get; set; }

        public List<ProjectDeadline> Deadlines { get; set; }

        public List<MessageViewModel> Messages { get; set; }

        // Number of unread messages for current user
        public int UnreadCount { get; set; }

        // For real-time presence display (SignalR)
        public List<string> OnlineUsers { get; set; } = new List<string>();
    }
}