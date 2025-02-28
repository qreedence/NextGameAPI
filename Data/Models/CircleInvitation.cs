namespace NextGameAPI.Data.Models
{
    public class CircleInvitation
    {
        public int Id { get; set; }
        public User From { get; set; }
        public User To { get; set; }
        public Circle Circle { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
