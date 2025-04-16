using NextGameAPI.Constants;

namespace NextGameAPI.DTOs
{
    public class GameVoteDTO
    {
        public int Id { get; set; }
        public required UserDTO User { get; set; }
        public GameVoteStatus Status { get; set; }
    }
}
