using Microsoft.AspNetCore.SignalR;

namespace SportsManagementApp.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinOpsGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ops");
        }

        public async Task JoinAdminGroup(int adminId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"admin:{adminId}");
        }
    }
}