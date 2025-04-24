using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class InvolvedCompanyDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("company")]
        public CompanyDTO CompanyId { get; set; }
        [JsonPropertyName("developer")]
        public bool Developer { get; set; }
        [JsonPropertyName("porting")]
        public bool Porting { get; set; }
        [JsonPropertyName("publisher")]
        public bool Publisher { get; set; }
        [JsonPropertyName("supporting")]
        public bool Supporting { get; set; }
    }
}
