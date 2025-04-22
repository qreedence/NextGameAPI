using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameDetailsResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("first_release_date")]
        public long FirstReleaseDate { get; set; }
        [JsonPropertyName("game_modes")]
        public List<int> GameModes { get; set; } = [];
        [JsonPropertyName("genres")]
        public List<int> GenreIds { get; set; } = [];
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("platforms")]
        public List<int> PlatformIds { get; set; } = [];
        [JsonPropertyName("slug")]
        public string Slug { get; set; } = "";
        [JsonPropertyName("storyline")]
        public string Storyline { get; set; } = "";
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";
        [JsonPropertyName("updated_at")]
        public long UpdatedAt { get; set; }
    }
}
