using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs
{
    public class FileUploadRequestPayload
    {
        [JsonPropertyName("files")]
        public List<FileUploadRequest> Files { get; set; }
    }
}
