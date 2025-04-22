namespace NextGameAPI.DTOs.Games
{
    public class FullGameResponse
    {
        public List<GameDetailsResponse> GameDetails { get; set; } = [];
        public List<GameCoverDTO> GameCover { get; set; } = [];
        public List<ScreenshotResponse> Screenshots { get; set; } = [];
        public List<VideoResponse> Videos { get; set; } = [];
        public List<InvolvedCompanyDTO> InvolvedCompanies { get; set; } = [];
        public List<MultiplayerModesDTO> MultiplayerModes { get; set; } = [];
        public List<WebsiteResponse> Websites { get; set; } = [];
    }
}
