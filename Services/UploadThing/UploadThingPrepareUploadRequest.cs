using System.Text.Json.Serialization;

namespace NextGameAPI.Services.UploadThing
{
    public class UploadThingPrepareUploadRequest
    {
        [JsonPropertyName("fileName")]
        public string Filename { get; set; }
        [JsonPropertyName("fileSize")]
        public float Filesize { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("fileType")]
        public string Filetype { get; set; }
        [JsonPropertyName("expiresIn")]
        public long ExpiresIn { get; set; }
    }
}
