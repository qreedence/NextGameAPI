namespace NextGameAPI.Data.Models
{
    public class GameSuggestion
    {
        public int Id { get; set; }
        public Guid CircleId { get; set; }
        public int GameId { get; set; }
        public List<GameVote> Votes { get; set; } = [];
    }
}
