using NextGameAPI.Constants;
using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs.Circles
{
    public class AddGameToCircleRequestDTO
    {
        public Guid CircleId { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; } = "";
        public string GameCoverUrl { get; set; } = "";
        public List<string> Players { get; set; } = [];
        public GameStatus GameStatus { get; set; }
        public required string SuggestedBy { get; set; }
    }
}
