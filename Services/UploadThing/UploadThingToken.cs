using System.Text.Json.Serialization;

namespace NextGameAPI.Services.UploadThing
{
    public class UploadThingToken
    {
        [JsonPropertyName("appId")]
        public string AppId { get; set; }
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }
        [JsonPropertyName("regions")]
        public List<string> Regions { get; set; }
    }
}
