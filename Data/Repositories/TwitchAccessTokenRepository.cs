using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class TwitchAccessTokenRepository : ITwitchAccessToken
    {
        private readonly ApplicationDbContext _applicationDbContext;
        
        public TwitchAccessTokenRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(TwitchAccessToken token)
        {
            if (token != null)
            {
                await _applicationDbContext.TwitchAccessTokens.AddAsync(token);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task<TwitchAccessToken?> GetAccessTokenAsync()
        {
            var token = await _applicationDbContext.TwitchAccessTokens.FirstOrDefaultAsync();
            if (token != null)
            {
                return token;
            }
            return null;
        }

        public async Task UpdateAsync(TwitchAccessToken token)
        {
            if (token != null)
            {
                _applicationDbContext.TwitchAccessTokens.Update(token);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
