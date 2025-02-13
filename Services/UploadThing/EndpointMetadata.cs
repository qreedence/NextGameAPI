using System.Text.Json.Serialization;

namespace NextGameAPI.Services.UploadThing
{
    public class EndpointMetadata
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("config")]
        public List<string> Endpoints { get; set; }
    }
}
