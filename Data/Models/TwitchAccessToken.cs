namespace NextGameAPI.Data.Models
{
    public class TwitchAccessToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
