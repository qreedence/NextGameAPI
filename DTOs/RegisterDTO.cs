namespace NextGameAPI.DTOs
{
    public class RegisterDTO
    {
        public required string Email {  get; set; }
        public required string UserName {  get; set; }
        public required string Password { get; set; }
    }
}
