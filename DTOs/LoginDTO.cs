namespace NextGameAPI.DTOs
{
    public class LoginDTO
    {
        public required string UserNameOrEmail { get; set; }
        public required string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
