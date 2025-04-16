using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs
{
    public class GameSuggestionDTO
    {
        public int Id { get; set; }
        public Guid CircleId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; } = "";
        public string GameCoverUrl { get; set; } = "";
        public string SuggestedBy { get; set; } = "";
        public List<GameVoteDTO> Votes { get; set; } = [];
    }
}
