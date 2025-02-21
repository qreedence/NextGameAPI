using NextGameAPI.Data.Models;

namespace NextGameAPI.Services.Notifications
{
    public interface ISignalRNotificationDispatcher
    {
        Task SendNotification(string username, Notification notification);
    }
}
