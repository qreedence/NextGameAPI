using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class VideoResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("video_id")]
        public string VideoId { get; set; } = "";
    }
}
