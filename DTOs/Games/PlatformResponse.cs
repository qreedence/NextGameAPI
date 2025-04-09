using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class PlatformResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; } = "";
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }
}
