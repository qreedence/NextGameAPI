using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Services.Notifications
{
    public class NotificationService
    {
        private readonly INotification _notificationRepo;
        private readonly ISignalRNotificationDispatcher _notificationDispatcher;

        public NotificationService(INotification notificationRepo, ISignalRNotificationDispatcher notificationDispatcher)
        {
            _notificationRepo = notificationRepo;
            _notificationDispatcher = notificationDispatcher;
        }

        public async Task SendNotificationAsync(User user, Notification notification)
        {
            if (user != null && !string.IsNullOrEmpty(user.UserName))
            {
                await _notificationDispatcher.SendNotification(user.UserName, notification);
            }
        }

        public async Task<Notification> CreateFriendRequestNotificationAsync(User from, User to)
        {
            if (from == null || to == null)
            {
                return null;
            }

            var notification = new Notification { 
                Type = NotificationType.FriendRequest, 
                User = to, 
                Data = $"You have received a friend request from {from.UserName}" };

            await _notificationRepo.CreateNotification(notification);
            return notification;
        }
    }
}
