using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;
using System.Security.Cryptography;

namespace NextGameAPI.Data.Repositories
{
    public class CircleRepository : ICircle
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CircleRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task CreateCircleAsync(Circle circle)
        {
            if (circle != null)
            {
                await _applicationDbContext.Circles.AddAsync(circle);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteCircleByIdAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                var circle = await GetByIdAsync(id);
                if (circle != null)
                {
                    _applicationDbContext.Circles.Remove(circle);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<Circle> GetByIdAsync(Guid id)
        {
            if (id != Guid.Empty)
            {
                var circle = _applicationDbContext.Circles.Local.FirstOrDefault(c => c.Id == id);
                if (circle != null)
                {
                    return circle;
                }

                circle = await _applicationDbContext.Circles
                    .Include(c => c.CreatedBy)
                        .ThenInclude(cb => cb.Settings)
                    .Include(c => c.CircleMembers.Where(cm => cm.IsActive))
                        .ThenInclude(cm => cm.User)
                            .ThenInclude(cm => cm.Settings)
                    .Include(c => c.SuggestionQueue)
                        .ThenInclude(sq => sq.Votes)
                    .Include(c => c.CircleGames)
                    .Where(c => c.CircleMembers
                        .Any(cm => cm.IsActive))
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (circle != null)
                {
                    return circle;
                }
            }
            return null;
        }

        public async Task<List<Circle>> GetCirclesByUserId(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var circles = await _applicationDbContext.Circles
                    .Include(c => c.CreatedBy)
                    .Include(c => c.SuggestionQueue)
                        .ThenInclude(sq => sq.Votes)
                    .Include(c => c.CircleGames)
                    .Include(c => c.CircleMembers.Where(cm => cm.IsActive))
                        .ThenInclude(cm => cm.User)
                    .ThenInclude(cm => cm.Settings)
                    .Where(c => c.CircleMembers
                        .Any(cm => cm.User.Id == userId && cm.IsActive))
                    .ToListAsync();
                if (circles != null && circles.Count > 0)
                {
                    return circles;
                }
            }
            return new List<Circle>();
        }

        public async Task UpdateCircleAsync(Circle circle)
        {
            if (circle != null)
            {
                try
                {
                _applicationDbContext.Circles.Update(circle);
                await _applicationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
