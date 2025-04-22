using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class CircleGameRepository : ICircleGame
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CircleGameRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(CircleGame circleGame)
        {
            if (circleGame != null)
            {
                var existingGame = _applicationDbContext.CircleGames.Any(cg => cg.GameId == circleGame.GameId && cg.Circle.Id == circleGame.Circle.Id);
                if (existingGame == false)
                {
                    await _applicationDbContext.CircleGames.AddAsync(circleGame);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<CircleGame?> GetByIdAsync(int id)
        {
            return await _applicationDbContext.CircleGames.FirstOrDefaultAsync(cg => cg.Id == id);
        }

        public async Task UpdateAsync(CircleGame circleGame)
        {
            if ( circleGame != null)
            {
                _applicationDbContext.CircleGames.Update(circleGame);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
