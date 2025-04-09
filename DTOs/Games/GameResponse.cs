using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("aggregated_rating")]
        public double AggregatedRating { get; set; }
        [JsonPropertyName("cover")]
        public int Cover { get; set; }
        [JsonPropertyName("genres")]
        public List<int> Genres { get; set; } = [];
        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }
        [JsonPropertyName("multiplayer_modes")]
        public List<int> MultiplayerModes{ get; set; } = [];
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("platforms")]
        public List<int> Platforms { get; set; } = [];
        [JsonPropertyName("screenshots")]
        public List<int> Screenshots { get; set; } = [];
        [JsonPropertyName("similar_games")]
        public List<int> SimilarGames { get; set; } = [];
        [JsonPropertyName("storyline")]
        public string Storyline { get; set; } = "";
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";
        [JsonPropertyName("videos")]
        public List<int> Videos { get; set; } = [];
        [JsonPropertyName("websites")]
        public List<int> Websites { get; set; } = [];
    }
}
