using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Services.Notifications
{
    public class NotificationService
    {
        private readonly INotification _notificationRepo;
        private readonly ISignalRNotificationDispatcher _notificationDispatcher;
        private readonly IUserSettings _userSettingsRepo;

        public NotificationService(INotification notificationRepo, ISignalRNotificationDispatcher notificationDispatcher, IUserSettings userSettingsRepo)
        {
            _notificationRepo = notificationRepo;
            _notificationDispatcher = notificationDispatcher;
            _userSettingsRepo = userSettingsRepo;
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

            var settings = await _userSettingsRepo.GetUserSettingsDTOByUserIdAsync(from.Id);
            string avatarUrl = "";
            if (settings != null)
            {
                avatarUrl = settings.Avatar;
            }

            var notification = new Notification
            {
                Type = NotificationType.FriendRequest,
                User = to,
                Data = $"You have received a friend request from {from.UserName}",
                ActionUrl = $"/u/{from.UserName}",
                AvatarUrl = avatarUrl,
            };

            await _notificationRepo.CreateNotification(notification);
            return notification;
        }
    }
}
