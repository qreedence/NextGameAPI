using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class FranchiseGameDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("cover")]
        public GameCoverDTO Cover { get; set; }
        //[JsonPropertyName("parent_game")]
        //public int? ParentGame { get; set; }
        //[JsonPropertyName("version_parent")]
        //public int? VersionParent { get; set; }
        [JsonPropertyName("first_release_date")]
        public long FirstReleaseDate { get; set; }
    }
}
