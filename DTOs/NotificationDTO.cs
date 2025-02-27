using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public required NotificationType Type { get; set; }
        public required string Data { get; set; }
        public string? AvatarUrl { get; set; }
        public string ActionUrl { get; set; } = "";
        public bool Seen { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
