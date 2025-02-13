using System.Text.Json.Serialization;

namespace NextGameAPI.Services.UploadThing
{
    public class RouteConfig
    {
        [JsonPropertyName("maxFileSize")]
        public string MaxFileSize { get; set; }

        [JsonPropertyName("maxFileCount")]
        public int MaxFileCount { get; set; }

        [JsonPropertyName("minFileCount")]
        public int MinFileCount { get; set; }

        [JsonPropertyName("contentDisposition")]
        public string ContentDisposition { get; set; }

        [JsonPropertyName("acl")]
        public string Acl { get; set; }
    }
}
