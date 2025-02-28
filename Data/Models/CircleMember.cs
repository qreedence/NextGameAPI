using NextGameAPI.Constants;

namespace NextGameAPI.Data.Models
{
    public class CircleMember
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Circle Circle { get; set; }
        public required User User { get; set; }
        public CircleMemberRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LeftAt { get; set; }
    }
}
