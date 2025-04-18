using NextGameAPI.Constants;
using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs.Circles
{
    public class CircleGameDTO
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string GameName { get; set; } = "";
        public string GameCoverUrl { get; set; } = "";
        public List<UserDTO> Players { get; set; } = [];
        public int DisplayOrder { get; set; }
        public GameStatus GameStatus { get; set; }
        public DateTime DateAdded { get; set; } 
        public DateTime DateStarted { get; set; }
        public DateTime DateFinished { get; set; }
        public List<DateTime> DatesPlayed { get; set; } = [];
        public required UserDTO SuggestedBy { get; set; }
    }
}
