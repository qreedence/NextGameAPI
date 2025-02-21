using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface INotification
    {
        Task CreateNotification (Notification notification);
        Task<List<Notification>> GetNotificationsForUser(User user);
    }
}
