using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs.Circles
{
    public class CircleDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required UserDTO CreatedBy { get; set; }
        public required DateTime CreatedAt { get; set; }
        public List<CircleMemberDTO> ActiveMembers { get; set; } = [];
        public List<GameSuggestionDTO> SuggestionQueue { get; set; } = [];
        public List<CircleGameDTO> CircleGames { get; set; } = [];
    }
}
