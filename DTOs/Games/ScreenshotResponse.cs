using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class ScreenshotResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("image_id")]
        public string ImageId { get; set; } = "";
    }
}
