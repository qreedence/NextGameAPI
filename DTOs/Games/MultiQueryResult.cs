using System.Text.Json.Serialization;
using System.Text.Json;

namespace NextGameAPI.DTOs.Games
{
    public class MultiQueryResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("result")]
        public JsonElement Result { get; set; } // Use JsonElement for the result
    }
}
