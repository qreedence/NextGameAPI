using Azure.Core;
using NextGameAPI.DTOs.Games;
using System.Net.Http;
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

        private async Task EnsureAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                _accessToken = await _twitterAccessTokenService.GetAccessTokenAsync();
            }
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");
        }
        public async Task<List<GameSearchResultDTO>> SearchGamesAsync(string searchTerm)
        {
            string queryBody = $@"
                search ""{searchTerm}"";
                fields id, name, cover, first_release_date;
                limit 10;
            ";
            return await GetGameList(queryBody);
        }

        private async Task<List<GameSearchResultDTO>> GetGameList(string queryBody)
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
    }
}
