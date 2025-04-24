using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class ParentGameDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("slug")]
        public string Slug { get; set; } = "";
        [JsonPropertyName("cover")]
        public GameCoverDTO Cover { get; set; } 
    }
}
