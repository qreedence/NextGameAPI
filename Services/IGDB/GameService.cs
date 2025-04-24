using NextGameAPI.Constants;
using NextGameAPI.DTOs.Games;
using System;
using System.Text;
using System.Text.Json;

namespace NextGameAPI.Services.IGDB
{
    public class GameService
    {
        private readonly TwitchAccessTokenService _twitterAccessTokenService;
        private readonly HttpClient _httpClient;
        private string _accessToken = "";

        public GameService (TwitchAccessTokenService twitchAccessTokenService, IHttpClientFactory httpClientFactory)
        {
            _twitterAccessTokenService = twitchAccessTokenService;
            _httpClient = httpClientFactory.CreateClient("IGDBClient");
        }
        #region Public Methods
        public async Task<List<GameSearchResultDTO>> SearchGamesAsync(string searchTerm)
        {
            string queryBody = $@"
                search ""{searchTerm}"";
                fields id, name, cover.image_id, first_release_date;
                limit 10;
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<List<GameSearchResultDTO>> GetNewGamesAsync()
        {
            string queryBody = $@"
                fields id, name, cover.image_id, aggregated_rating, first_release_date;
                where aggregated_rating > 60 & first_release_date < {(int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()};
                sort first_release_date desc;
                limit 10;
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<List<GameSearchResultDTO>> GetAllNewGamesAsync(int page = 1, int pageSize = 50)
        {
            pageSize = Math.Clamp(pageSize, 1, 50);

            int offset = (page - 1) * pageSize;

            string queryBody = $@"
                fields id, name, cover.image_id, first_release_date;
                where first_release_date < {(int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()};
                sort first_release_date desc;
                limit {pageSize};
                offset {offset};
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<List<GameSearchResultDTO>> GetHighestRatedGamesOfYear(int year)
        {
            DateTimeOffset startOfYear = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            long startUnixTime = startOfYear.ToUnixTimeSeconds();

            DateTimeOffset endOfYear = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
            long endUnixTime = endOfYear.ToUnixTimeSeconds();

            string queryBody = $@"
                fields id, name, cover.image_id, aggregated_rating, total_rating, first_release_date, aggregated_rating_count;
                where first_release_date >= {startUnixTime} & first_release_date < {endUnixTime} & aggregated_rating_count > 1;
                sort total_rating desc;
                limit 10;
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<List<GameSearchResultDTO>> GetAllHighestRatedGamesOfYear(int year, int page = 1, int pageSize = 50)
        {
            DateTimeOffset startOfYear = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero);
            long startUnixTime = startOfYear.ToUnixTimeSeconds();

            DateTimeOffset endOfYear = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, TimeSpan.Zero);
            long endUnixTime = endOfYear.ToUnixTimeSeconds();
            pageSize = Math.Clamp(pageSize, 1, 50);

            int offset = (page - 1) * pageSize;

            string queryBody = $@"
                fields id, name, cover.image_id, aggregated_rating, total_rating, first_release_date, aggregated_rating_count;
                where first_release_date >= {startUnixTime} & first_release_date < {endUnixTime} & aggregated_rating_count > 1;
                sort total_rating desc;
                limit {pageSize};
                offset {offset};
            ";
            return await GetGameList("games", queryBody);
        }

        //New get game query
        public async Task<GameDTO?> GetGameAsync(string gameId)
        {
            string queryBody = $@"
                fields 
                    total_rating, aggregated_rating,     
                    cover.image_id, 
                    first_release_date, updated_at,
                    genres.name, 
                    multiplayer_modes, multiplayer_modes.campaigncoop, multiplayer_modes.dropin, multiplayer_modes.lancoop, multiplayer_modes.offlinecoop, multiplayer_modes.offlinecoopmax, multiplayer_modes.offlinemax, multiplayer_modes.onlinecoop, multiplayer_modes.onlinecoopmax, multiplayer_modes.onlinemax, multiplayer_modes.splitscreen, multiplayer_modes.splitscreenonline,
                    name,
                    themes.name,
                    platforms.abbreviation, 
                    screenshots.image_id, 
                    storyline, 
                    summary, 
                    videos.video_id, 
                    websites.type, websites.url,
                    game_modes.name, 
                    involved_companies, involved_companies.developer, involved_companies.publisher, involved_companies.supporting, involved_companies.porting, involved_companies.company.name,
                    slug, 
                    franchises.name, franchises.games.name, franchises.games.cover.image_id, franchises.games.first_release_date, franchises.games.parent_game, franchises.games.version_parent,
                    keywords.name,
                    parent_game.name, parent_game.cover.url;
                where id = {gameId};
            ";
            return await SendGetGameRequestAsync(queryBody);
        }

        #endregion
        #region Game Requests

        private async Task<GameDTO?> SendGetGameRequestAsync(string queryBody)
        {
            await EnsureAccessToken();
            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("games", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var gameList = JsonSerializer.Deserialize<List<GameResponse>>(responseContent);
            if (gameList != null &&  gameList.Count > 0)
            {
                var gameResponse = gameList.FirstOrDefault();
                if (gameResponse == null)
                {
                    return null;
                }
                var game = await ConvertGameResponseToGameDTO(gameResponse);
                
                return game;
            }
            return null;
        }
        private async Task<List<GameSearchResultDTO>> GetGameList(string endpoint, string queryBody)
        {
            await EnsureAccessToken();
            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("games", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var gameList = JsonSerializer.Deserialize<List<GameSearchResultDTO>>(responseContent);
            if (gameList != null && gameList.Count > 0)
            {
                foreach (var game in gameList)
                {
                    if (game.Cover != null && !string.IsNullOrEmpty(game.Cover.ImageId))
                    {
                        game.CoverUrl = GetFormattedImageLink(game.Cover.ImageId, ImageType.Cover);
                    }
                    if (game.FirstReleaseDateUnix.HasValue)
                    {
                        game.FirstReleaseDate = DateTimeOffset.FromUnixTimeSeconds(game.FirstReleaseDateUnix.Value).UtcDateTime.Date;
                    }
                }
            }
            return gameList;
        }
        #endregion
        #region Game Attribute Requests
        
        private GameLinks GetWebsites(List<WebsiteResponse> websiteLinks)
        {

            var gameLinks = new GameLinks();

            if (websiteLinks == null || websiteLinks.Count <= 0)
            {
                return gameLinks;
            }
            foreach (var website in websiteLinks)
            {
                switch (website.WebsiteType)
                {
                    case WebsiteType.Official:
                        gameLinks.Official = website.Url;
                        break;
                    case WebsiteType.Steam:
                        gameLinks.Steam = website.Url;
                        break;
                    case WebsiteType.Epicgames:
                        gameLinks.EpicGames = website.Url;
                        break;
                    case WebsiteType.Gog:
                        gameLinks.GOG = website.Url;
                        break;
                }
            }
            return gameLinks;
        }

        #endregion
        #region Private Helper Methods
        private async Task<GameDTO> ConvertGameResponseToGameDTO(GameResponse gameResponse)
        {
            if (gameResponse == null)
            {
                return null;
            }

            var game = new GameDTO();

            game.Id = gameResponse.Id;
            game.AggregatedRating = gameResponse.AggregatedRating;
            game.TotalRating = gameResponse.TotalRating;
            if (gameResponse.Cover != null && !string.IsNullOrEmpty(gameResponse.Cover.ImageId))
            {
                game.CoverUrl = GetFormattedImageLink(gameResponse.Cover.ImageId, ImageType.Cover);
            }
            if (gameResponse.FirstReleaseDate.HasValue)
            {
                game.FirstReleaseDate = DateTimeOffset.FromUnixTimeSeconds(gameResponse.FirstReleaseDate.Value).UtcDateTime.Date;
            }
            foreach (var franchise in gameResponse.Franchises)
            {
                franchise.Games = franchise.Games
                                           //.Where(franchiseGame => franchiseGame.ParentGame == null && franchiseGame.VersionParent == null)
                                           .OrderByDescending(franchiseGame => franchiseGame.FirstReleaseDate)
                                           .ToList();
            }
            game.Franchises = gameResponse.Franchises;
            game.GameModes = gameResponse.GameModes.Select(x => x.Name).ToList();
            game.Genres = gameResponse.Genres.Select(x => x.Name).ToList();
            game.InvolvedCompanies = gameResponse.InvolvedCompanies;
            game.Keywords = gameResponse.Keywords.Select(x => x.Name).ToList();
            game.MultiplayerModes = gameResponse.MultiplayerModes != null ? gameResponse.MultiplayerModes.FirstOrDefault() : new MultiplayerModesDTO();
            game.Name = gameResponse.Name;
            game.ParentGame = gameResponse.ParentGame;
            game.Platforms = gameResponse.Platforms.Select(x => x.Abbreviation).ToList();
            game.Screenshots = GetFormattedImageLinks(gameResponse.Screenshots.Select(x => x.ImageId).ToList(), ImageType.Screenshot);
            game.Slug = gameResponse.Slug;
            game.Storyline = gameResponse.Storyline;
            game.Summary = gameResponse.Summary;
            game.Themes = gameResponse.Themes.Select(x => x.Name).ToList();
            if (gameResponse.FirstReleaseDate.HasValue)
            {
                game.UpdatedAt = DateTimeOffset.FromUnixTimeSeconds(gameResponse.UpdatedAt.Value).UtcDateTime.Date;
            }
            game.Videos = gameResponse.Videos.Select(x => x.VideoId).ToList();
            game.Websites = GetWebsites(gameResponse.Websites); 

            return game;
        }
        private async Task EnsureAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await _twitterAccessTokenService.GetAccessTokenAsync();
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }

        private string GetFormattedImageLink(string imageId, ImageType imageType)
        {
            string type = "";
            switch (imageType)
            {
                case ImageType.Cover:
                    type = "cover_big_2x";
                    break;
                default:
                    type = "1080p_2x";
                    break;
            }

            return $"https://images.igdb.com/igdb/image/upload/t_{type}/{imageId}.png";
        }

        private List<string> GetFormattedImageLinks(List<string> images, ImageType imageType)
        {
            var formattedLinks = new List<string>();
            foreach (var image in images)
            {
                var formattedLink = GetFormattedImageLink(image, imageType);
                formattedLinks.Add(formattedLink);
            }
            return formattedLinks;
        }

        private async Task<string> IntToStringRequests (string endpoint, string queryBody)
        {
            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync(endpoint, content);
            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}
