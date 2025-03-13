using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameCoverDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
