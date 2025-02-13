using Newtonsoft.Json;

namespace NextGameAPI.DTOs
{
    public class FileUploadRequest
    {
        [JsonProperty("lastModified")]
        public long LastModified { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("size")]
        public float Size { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
