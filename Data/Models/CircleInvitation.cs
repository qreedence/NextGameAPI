namespace NextGameAPI.Data.Models
{
    public class CircleInvitation
    {
        public int Id { get; set; }
        public required User From { get; set; }
        public required User To { get; set; }
        public required Circle Circle { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
