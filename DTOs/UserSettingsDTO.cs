namespace NextGameAPI.DTOs
{
    public class UserSettingsDTO
    {
        public required string UserId { get; set; }
        public string Avatar { get; set; } = "";
        public bool AccountIsPublic { get; set; }
        public required string UserName { get; set; }
        public bool HasPassword { get; set; }
    }
}
