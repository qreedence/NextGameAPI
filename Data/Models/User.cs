using Microsoft.AspNetCore.Identity;

namespace NextGameAPI.Data.Models
{
    public class User : IdentityUser
    {
        public DateTime RegisterDate { get; set;  } = DateTime.UtcNow;
        public UserSettings Settings { get; set; } = new UserSettings();
        public List<SocialLink> SocialLinks { get; set; } = new List<SocialLink>();
    }
}
