using Google.Apis.Auth.OAuth2.Responses;
using NextGameAPI.Data;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using System.Net.Http;

namespace NextGameAPI.Services.IGDB
{
    public class TwitchAccessTokenService
    {
        private readonly ITwitchAccessToken _twitchAccessTokenRepository;
        private HttpClient _httpClient;

        public TwitchAccessTokenService(ITwitchAccessToken twitchAccessTokenRepository, IHttpClientFactory httpClientFactory)
        {
            _twitchAccessTokenRepository = twitchAccessTokenRepository;
            _httpClient = httpClientFactory.CreateClient("IGDBAuthClient");
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var token = await _twitchAccessTokenRepository.GetAccessTokenAsync();
            if (token == null || token.Expiry < DateTime.UtcNow.AddHours(1))
            {
                token = await GenerateNewTokenAsync();
            }
            return token.Token;
        }

        private async Task<TwitchAccessToken> GenerateNewTokenAsync()
        {
            var values = new Dictionary<string, string>
            {
                {"client_id", Environment.GetEnvironmentVariable("igdb-client-id") },
                {"client_secret", Environment.GetEnvironmentVariable("igdb-client-secret") },
                {"grant_type", "client_credentials" }
            };
            var response = await _httpClient.PostAsync("token", new FormUrlEncodedContent(values));
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (tokenResponse != null)
            {
                var newToken = new TwitchAccessToken
                {
                    Token = tokenResponse.AccessToken,
                    Expiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
                };

                var oldToken = await _twitchAccessTokenRepository.GetAccessTokenAsync();
                if (oldToken != null)
                {
                    oldToken.Token = newToken.Token;
                    oldToken.Expiry = newToken.Expiry;
                    await _twitchAccessTokenRepository.UpdateAsync(oldToken);
                }
                else
                {
                    await _twitchAccessTokenRepository.AddAsync(newToken);
                }
                return newToken;
            }
            throw new Exception("Failed to retrieve access token");
    }
    }
}
