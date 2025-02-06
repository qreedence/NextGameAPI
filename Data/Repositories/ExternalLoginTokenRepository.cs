using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class ExternalLoginTokenRepository : IExternalLoginToken
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ExternalLoginTokenRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ExternalLoginToken> GetByIdAsync(Guid id)
        {
            var result = await _applicationDbContext.ExternalLoginTokens.FirstOrDefaultAsync(x => x.Id == id);
            return result ?? null;
        }

        public async Task Add(ExternalLoginToken externalLoginToken)
        {
            DeleteAllOldTokens();
            await _applicationDbContext.ExternalLoginTokens.AddAsync(externalLoginToken);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(ExternalLoginToken token)
        {
            if (token != null)
            {
                _applicationDbContext.ExternalLoginTokens.Remove(token);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        private void DeleteAllOldTokens()
        {
            var oldTokens = _applicationDbContext.ExternalLoginTokens.Where(token => token.Expiry <  DateTime.UtcNow).ToList();
            _applicationDbContext.ExternalLoginTokens.RemoveRange(oldTokens);
        }
    }
}
