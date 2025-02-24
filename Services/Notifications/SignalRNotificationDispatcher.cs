using Microsoft.AspNetCore.SignalR;
using NextGameAPI.Data.Models;
using NextGameAPI.Hubs;

namespace NextGameAPI.Services.Notifications
{
    public class SignalRNotificationDispatcher : ISignalRNotificationDispatcher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationDispatcher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;   
        }
        public async Task SendNotification(string username, Notification notification)
        {
            await _hubContext.Clients.Group($"user_{username}")
            .SendAsync("NotificationsUpdated", notification);
        }
    }
}
