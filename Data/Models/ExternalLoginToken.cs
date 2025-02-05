using Microsoft.AspNetCore.Identity;

namespace NextGameAPI.Data.Models
{
    public class ExternalLoginToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string LoginProvider { get; set; } = "";
        public string ProviderKey { get; set; } = "";
        public DateTime Expiry { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }
}
