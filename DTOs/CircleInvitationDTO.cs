using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs
{
    public class CircleInvitationDTO
    {
        public int Id { get; set; }
        public required UserDTO From { get; set; }
        public required CircleDTO Circle { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
