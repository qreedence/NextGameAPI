using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetToken
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public PasswordResetTokenRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<PasswordResetToken> GetById(string id)
        {
            return await _applicationDbContext.PasswordResetTokens.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddPasswordResetToken(PasswordResetToken token)
        {
            await _applicationDbContext.PasswordResetTokens.AddAsync(token);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task RemovePasswordResetToken(string id)
        {
            var token = await GetById(id);
            _applicationDbContext.PasswordResetTokens.Remove(token);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
