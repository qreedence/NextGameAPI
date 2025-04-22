using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameDTO
    {
        public int Id { get; set; }
        public double AggregatedRating { get; set; }
        public string? CoverUrl { get; set; }
        public List<string> Genres { get; set; } = [];
        public DateTime FirstReleaseDate { get; set; }
        public MultiplayerModesDTO MultiplayerModes { get; set; } = new MultiplayerModesDTO();
        public string Name { get; set; } = "";
        public List<string> Platforms { get; set; } = [];
        public List<string> Screenshots { get; set; } = [];
        public string Storyline { get; set; } = "";
        public string Summary { get; set; } = "";
        public List<string> Videos { get; set; } = [];
        public GameLinks Websites { get; set; } = new GameLinks();
        public List<string> SimilarGames { get; set; } = [];
        public DateTime UpdatedAt { get; set; }
        public List<CompanyDTO> InvolvedCompanies { get; set; } = [];
        public List<string> GameModes { get; set; } = [];
    }
}
