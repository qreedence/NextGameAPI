using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class GameVoteRepository : IGameVote
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GameVoteRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(GameVote gameVote)
        {
            if (gameVote == null)
            {
                return;
            }

            await _applicationDbContext.GameVotes.AddAsync(gameVote);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<GameVote?> GetByIdAsync(int id)
        {
            return await _applicationDbContext.GameVotes
                .Include(v => v.User)
                    .ThenInclude(u => u.Settings)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(GameVote gameVote)
        {
            if (gameVote == null)
            {
                return;
            }

            _applicationDbContext.GameVotes.Update(gameVote);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
