using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class ThemeResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
