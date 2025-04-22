namespace NextGameAPI.Data.Models
{
    public class WishlistedGame
    {
        public int Id { get; set; }
        public required User User { get; set; }
        public int GameId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
