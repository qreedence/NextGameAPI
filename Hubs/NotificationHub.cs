using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace NextGameAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (username != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{username}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name;
            if (username != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{username}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
