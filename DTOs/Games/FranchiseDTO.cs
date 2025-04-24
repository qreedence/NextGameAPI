using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class FranchiseDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("games")]
        public List<FranchiseGameDTO> Games { get; set; } = [];
    }
}
