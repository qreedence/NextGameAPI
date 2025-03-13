using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ITwitchAccessToken
    {
        Task<TwitchAccessToken?> GetAccessTokenAsync();
        Task AddAsync (TwitchAccessToken twitchAccessToken);
        Task UpdateAsync(TwitchAccessToken token);
    }
}
