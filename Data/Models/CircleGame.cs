using NextGameAPI.Constants;

namespace NextGameAPI.Data.Models
{
    public class CircleGame
    {
        public int Id { get; set; } 
        public int GameId { get; set; }
        public Circle Circle { get; set; }
        public string GameName { get; set; } = "";
        public string GameCoverUrl { get; set; } = "";
        public List<CircleMember> Players { get; set; } = []; 
        public int DisplayOrder { get; set; }
        public GameStatus GameStatus { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public DateTime DateStarted { get; set; }
        public DateTime DateFinished { get; set; }
        public List<DateTime> DatesPlayed { get; set; } = [];
        public CircleMember? SuggestedBy { get; set; }
    }
}
