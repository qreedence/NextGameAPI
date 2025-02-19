namespace NextGameAPI.Data.Models
{
    public class PasswordResetToken
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Token { get; set; } = "";
        public string UserId { get; set; } = "";
    }
}
