namespace NextGameAPI.Data.Models
{
    public class UserSettings
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public int Id { get; set; }
        public string Avatar { get; set; } = "";
        public string AvatarFileKey { get; set; } = "";
        public bool AccountIsPublic { get; set; } = true;
    }
}
