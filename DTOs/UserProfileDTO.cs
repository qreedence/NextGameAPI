namespace NextGameAPI.DTOs
{
    public class UserProfileDTO
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public string Avatar {  get; set; }
        public bool HasPassword { get; set; }
    }
}
