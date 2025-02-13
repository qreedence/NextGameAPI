using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs
{
    public class NewPresignedUrl
    {
        [JsonPropertyName("url")]
        public required string Url { get; set; }
        [JsonPropertyName("key")]
        public required string Key { get; set; }

    }
}
