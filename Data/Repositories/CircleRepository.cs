using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

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
                var circle = await _applicationDbContext.Circles.FirstOrDefaultAsync(x => x.Id == id);
                if (circle != null)
                {
                    return circle;
                }
            }
            return null;
        }

        public async Task UpdateCircleAsync(Circle circle)
        {
            if (circle != null)
            {
                _applicationDbContext.Circles.Update(circle);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
