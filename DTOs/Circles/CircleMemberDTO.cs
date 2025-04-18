using NextGameAPI.Constants;

namespace NextGameAPI.DTOs.Circles
{
    public class CircleMemberDTO
    {
        public required UserDTO User { get; set; }
        public required CircleMemberRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public Guid CircleId { get; set; }
    }
}
