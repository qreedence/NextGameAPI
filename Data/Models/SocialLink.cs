namespace NextGameAPI.Data.Models
{
    public class SocialLink
    {
        public int Id { get; set; }
        public required string Platform { get; set; }
        public required string Username { get; set; }
    }
}
