namespace NextGameAPI.Data.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public required User User { get; set; }
        public required NotificationType Type { get; set; }
        public required string Data { get; set; }
        public string AvatarUrl { get; set; } = "";
        public string ActionUrl { get; set; } = "";
        public bool Seen { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ViewedAt { get; set; }
    }

    public enum NotificationType
    {
        FriendRequest,
    }
}


