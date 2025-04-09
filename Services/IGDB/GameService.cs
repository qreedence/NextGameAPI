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
                fields id, name, cover, first_release_date;
                limit 10;
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<List<GameSearchResultDTO>> GetNewGamesAsync()
        {
            string queryBody = $@"
                fields id, name, cover, aggregated_rating, first_release_date;
                where aggregated_rating > 60 & first_release_date < {(int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()};
                sort first_release_date desc;
                limit 10;
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
                fields id, name, cover, aggregated_rating, first_release_date, aggregated_rating_count;
                where first_release_date >= {startUnixTime} & first_release_date < {endUnixTime} & aggregated_rating_count > 1;
                sort aggregated_rating desc;
                limit 10;
            ";
            return await GetGameList("games", queryBody);
        }

        public async Task<GameDTO?> GetGameAsync(string gameId)
        {
            string queryBody = $@"
                fields aggregated_rating, cover, first_release_date, franchise, genres, multiplayer_modes, name, platforms, screenshots, similar_games, storyline, summary, videos, websites;
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
                var coverIds = gameList
                    .Where(game => game.CoverID != null)
                    .Select(game => game.CoverID.Value)
                    .Distinct()
                    .ToList();
                var coverMap = await GetGameCoversInBatch(coverIds);
                foreach (var game in gameList)
                {
                    if (game.CoverID != null && coverMap.ContainsKey(game.CoverID.Value))
                    {
                        game.CoverUrl = coverMap[game.CoverID.Value];
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
        private async Task<string> GetCoverByIdAsync(int coverId)
        {
            string queryBody = $@"
                fields id, url;
                where id = ({coverId});
            ";

            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("covers", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var covers = JsonSerializer.Deserialize<List<GameCoverDTO>>(responseContent);
            
            if (covers == null || covers.Count <= 0)
            {
                return "";
            }
            return $"https://{covers.FirstOrDefault().Url!.Replace("thumb", "1080p").Substring(2)}";
        }
        private async Task<Dictionary<int, string>> GetGameCoversInBatch(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            string idList = string.Join(",", ids);
            string queryBody = $@"
                fields id, url;
                where id = ({idList});
            ";

            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("covers", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var covers = JsonSerializer.Deserialize<List<GameCoverDTO>>(responseContent);
            if (covers == null)
            {
                return new Dictionary<int, string>();
            }
            return covers.ToDictionary(
                cover => cover.Id,
                cover => string.IsNullOrEmpty(cover.Url)
                    ? ""
                    : $"https://{cover.Url.Replace("thumb", "1080p").Substring(2)}"
            );
        }
        private async Task<List<string>> GetGenresAsync(List<int> genreIds)
        {
            string idList = string.Join(",", genreIds);
            string queryBody = $@"
                fields name;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("genres", queryBody);
            var genres = JsonSerializer.Deserialize<List<GenreResponse>>(responseContent);
            if (genres == null || genres.Count <= 0)
            {
                return [];
            }
            var genreList = new List<string>();
            foreach (var genre in genres)
            {
                genreList.Add(genre.Name);
            }
            return genreList;
        }
        private async Task<MultiplayerModesDTO> GetMultiplayerModesAsync(List<int> multiplayerModeIds)
        {
            string idList = string.Join(",", multiplayerModeIds);
            string queryBody = $@"
                fields name;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("multiplayer_modes", queryBody);
            var multiplayerModesList = JsonSerializer.Deserialize<List<MultiplayerModesDTO>>(responseContent);
            if (multiplayerModesList == null || multiplayerModesList.Count <= 0 || multiplayerModesList.FirstOrDefault() == null)
            {
                return new MultiplayerModesDTO();
            }
            return multiplayerModesList.FirstOrDefault()!;
        }
        private async Task<List<string>> GetPlatformsAsync(List<int> platformIds)
        {
            string idList = string.Join(",", platformIds);
            string queryBody = $@"
                fields name, abbreviation;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("platforms", queryBody);
            var platforms = JsonSerializer.Deserialize<List<PlatformResponse>>(responseContent);
            if (platforms == null || platforms.Count <= 0)
            {
                return [];
            }
            var platformsList = new List<string>();
            foreach (var platform in platforms)
            {
                platformsList.Add(platform.Abbreviation);
            }
            return platformsList;
        }
        private async Task<List<string>> GetSimilarGamesAsync(List<int> gameIds)
        {
            string idList = string.Join(",", gameIds);
            string queryBody = $@"
                fields name;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("games", queryBody);
            var games = JsonSerializer.Deserialize<List<GameSearchResultDTO>>(responseContent);
            if (games == null || games.Count <= 0)
            {
                return [];
            }
            var gameList = new List<string>();
            foreach (var game in games)
            {
                gameList.Add(game.Name);
            }
            return gameList;
        }
        private async Task<List<string>> GetVideosAsync(List<int> videoIds)
        {
            string idList = string.Join(",", videoIds);
            string queryBody = $@"
                fields video_id;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("game_videos", queryBody);
            var videos = JsonSerializer.Deserialize<List<VideoResponse>>(responseContent);
            if (videos == null || videos.Count <= 0)
            {
                return [];
            }
            var videoList = new List<string>();
            foreach (var video in videos)
            {
                string videoUrl = $"youtube.com/watch?v={video.VideoUrl}";
                videoList.Add(videoUrl);
            }
            return videoList;
        }
        private async Task<List<string>> GetScreenshotsAsync(List<int> screenshotIds)
        {
            string idList = string.Join(",", screenshotIds);
            string queryBody = $@"
                fields url;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("screenshots", queryBody);
            var screenshots = JsonSerializer.Deserialize<List<ScreenshotResponse>>(responseContent);
            if (screenshots == null || screenshots.Count <= 0)
            {
                return [];
            }
            var screenshotList = new List<string>();
            foreach (var screenshot in screenshots)
            {
                string imageUrl = $"https://{screenshot.Url.Replace("thumb", "1080p").Substring(2)}";
                screenshotList.Add(imageUrl);
            }
            return screenshotList;
        }
        private async Task<GameLinks> GetWebsitesAsync(List<int> websiteLinks)
        {
            string idList = string.Join(",", websiteLinks);
            string queryBody = $@"
                fields id, type, trusted, url;
                where id = ({idList});
            ";
            var responseContent = await IntToStringRequests("websites", queryBody);
            var websites = JsonSerializer.Deserialize<List<WebsiteResponse>>(responseContent);
            var gameLinks = new GameLinks();
            
            if (websites == null || websites.Count <= 0)
            {
                return gameLinks;
            }
            foreach (var website in websites)
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
            game.Name = gameResponse.Name;
            game.Storyline = gameResponse.Storyline;
            game.Summary = gameResponse.Summary;
            game.CoverUrl = await GetCoverByIdAsync(gameResponse.Cover);
            if (gameResponse.FirstReleaseDate.HasValue)
            {
                game.FirstReleaseDate = DateTimeOffset.FromUnixTimeSeconds(gameResponse.FirstReleaseDate.Value).UtcDateTime.Date;
            }
            game.Genres = await GetGenresAsync(gameResponse.Genres);
            game.MultiplayerModes = await GetMultiplayerModesAsync(gameResponse.MultiplayerModes);
            game.Platforms = await GetPlatformsAsync(gameResponse.Platforms);
            game.Screenshots = await GetScreenshotsAsync(gameResponse.Screenshots);
            game.SimilarGames = await GetSimilarGamesAsync(gameResponse.SimilarGames);
            game.Videos = await GetVideosAsync(gameResponse.Videos);
            game.Websites = await GetWebsitesAsync(gameResponse.Websites);
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

        private async Task<string> IntToStringRequests (string endpoint, string queryBody)
        {
            var content = new StringContent(queryBody, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync(endpoint, content);
            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}
