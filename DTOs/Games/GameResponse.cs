using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("aggregated_rating")]
        public double AggregatedRating { get; set; }
        [JsonPropertyName("total_rating")]
        public double TotalRating { get; set; }
        [JsonPropertyName("cover")]
        public GameCoverDTO Cover { get; set; }
        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDate { get; set; }
        [JsonPropertyName("franchises")]
        public List<FranchiseDTO> Franchises { get; set; } = [];
        [JsonPropertyName("game_modes")]
        public List<GameModeResponse> GameModes { get; set; } = [];
        [JsonPropertyName("genres")]
        public List<GenreResponse> Genres { get; set; } = [];
        [JsonPropertyName("involved_companies")]
        public List<InvolvedCompanyDTO> InvolvedCompanies { get; set; } = [];
        [JsonPropertyName("keywords")]
        public List<KeywordResponse> Keywords { get; set; } = [];
        [JsonPropertyName("multiplayer_modes")]
        public List<MultiplayerModesDTO> MultiplayerModes { get; set; } = [];
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        [JsonPropertyName("parent_game")]
        public ParentGameDTO ParentGame { get; set; }
        [JsonPropertyName("platforms")]
        public List<PlatformResponse> Platforms { get; set; } = [];
        [JsonPropertyName("screenshots")]
        public List<ScreenshotResponse> Screenshots { get; set; } = [];
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("storyline")]
        public string Storyline { get; set; } = "";
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = "";
        [JsonPropertyName("themes")]
        public List<ThemeResponse> Themes { get; set; } = [];
        [JsonPropertyName("updated_at")]
        public long? UpdatedAt { get; set; }
        [JsonPropertyName("videos")]
        public List<VideoResponse> Videos { get; set; } = [];
        [JsonPropertyName("websites")]
        public List<WebsiteResponse> Websites { get; set; } = [];






        


       
        

        



    }
}
