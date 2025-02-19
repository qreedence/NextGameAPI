namespace NextGameAPI.Data.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public required User UserA { get; set; }
        public required User UserB { get; set; }
        public DateTime FriendsSince { get; set; } = DateTime.UtcNow;
    }
}