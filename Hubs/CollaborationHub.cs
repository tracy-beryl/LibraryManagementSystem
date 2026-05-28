using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Hubs
{
    public class CollaborationHub : Hub
    {
        // projectId -> (userId -> connectionIds)
        private static readonly ConcurrentDictionary<string,
            ConcurrentDictionary<string, HashSet<string>>> ProjectConnections
            = new ConcurrentDictionary<string,
                ConcurrentDictionary<string, HashSet<string>>>();

        private static readonly object _lock = new object();

        private string GetGroupName(string projectId)
            => $"Project-{projectId}";

        public async Task JoinProjectGroup(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return;

            var userId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(userId))
                return;

            var groupName = GetGroupName(projectId);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var projectUsers = ProjectConnections.GetOrAdd(
                projectId,
                _ => new ConcurrentDictionary<string, HashSet<string>>());

            lock (_lock)
            {
                if (!projectUsers.ContainsKey(userId))
                {
                    projectUsers[userId] = new HashSet<string>();
                }

                projectUsers[userId].Add(Context.ConnectionId);
            }

            await BroadcastOnlineUsers(projectId);
        }

        public async Task UserTyping(string projectId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return;

            var userId = Context.UserIdentifier;
            if (string.IsNullOrWhiteSpace(userId))
                return;

            await Clients.OthersInGroup(GetGroupName(projectId))
                .SendAsync("UserTyping", userId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                foreach (var projectEntry in ProjectConnections)
                {
                    var projectId = projectEntry.Key;
                    var users = projectEntry.Value;

                    bool userRemoved = false;

                    lock (_lock)
                    {
                        if (users.ContainsKey(userId))
                        {
                            users[userId].Remove(Context.ConnectionId);

                            if (users[userId].Count == 0)
                            {
                                users.TryRemove(userId, out _);
                                userRemoved = true;
                            }
                        }
                    }

                    if (userRemoved)
                    {
                        await BroadcastOnlineUsers(projectId);
                    }

                    if (!users.Any())
                    {
                        ProjectConnections.TryRemove(projectId, out _);
                    }
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private async Task BroadcastOnlineUsers(string projectId)
        {
            if (!ProjectConnections.ContainsKey(projectId))
                return;

            var onlineUsers = ProjectConnections[projectId]
                .Keys
                .ToList();

            await Clients.Group(GetGroupName(projectId))
                .SendAsync("OnlineUsersUpdated", onlineUsers);
        }
    }
}