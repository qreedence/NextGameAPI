using NextGameAPI.Constants;
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

            var notification = new Notification
            {
                Type = NotificationType.FriendRequest,
                User = to,
                Data = $"You have received a friend request from {from.UserName}.",
                ActionUrl = $"/u/{from.UserName}",
                AvatarUrl = await GetAvatar(from.Id),
            };

            await _notificationRepo.CreateNotification(notification);
            return notification;
        }

        public async Task<Notification> CreateCircleInvitationNotificationAsync(CircleInvitation circleInvitation)
        {
            if (circleInvitation == null)
            {
                return null;
            }

            var notification = new Notification
            {
                Type = NotificationType.CircleInvitation,
                User = circleInvitation.To,
                Data = $"{circleInvitation.From.UserName} has invited you to join {circleInvitation.Circle.Name}.",
                ActionUrl = "/placeholder",
                AvatarUrl = await GetAvatar(circleInvitation.From.Id),
            };

            await _notificationRepo.CreateNotification(notification);
            return notification;
        }

        private async Task<string> GetAvatar(string id)
        {
            var settings = await _userSettingsRepo.GetUserSettingsDTOByUserIdAsync(id);
            string avatarUrl = "";
            if (settings != null)
            {
                avatarUrl = settings.Avatar;
            }
            return avatarUrl;
        }
    }
}
