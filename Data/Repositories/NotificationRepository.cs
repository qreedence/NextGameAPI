using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class NotificationRepository : INotification
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public NotificationRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task CreateNotification(Notification notification)
        {
            if (notification == null)
            {
                return;
            }
            try
            {
                await _applicationDbContext.Notifications.AddAsync(notification);
                await _applicationDbContext.SaveChangesAsync();
            }
            catch (Exception) {
                return;
            }
        }

        public async Task MarkNotificationAsSeen(Guid id)
        {
            var notification = await _applicationDbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification != null && notification.Seen == false)
            {
                notification.Seen = true;
            }
            _applicationDbContext.Notifications.Update(notification);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsForUser(User user)
        {
            if (user != null)
            {
                var notifications = await _applicationDbContext.Notifications.Where(n => n.User == user).ToListAsync();
                if (notifications.Count() > 0)
                {
                    return notifications
                        .OrderByDescending(n => n.CreatedAt)
                        .Take(10)
                        .ToList();
                }
            }
            return new List<Notification>();
        }
    }
}
