using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class CircleInvitationRepository : ICircleInvitation
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CircleInvitationRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<CircleInvitation?> CheckExisting(User to, Guid circleId)
        {
            return await _applicationDbContext.CircleInvitations
                .Where(ci => ci.To.Id == to.Id && ci.Circle.Id == circleId)
                .FirstOrDefaultAsync();
        }

        public async Task<CircleInvitation> Create(User from, User to, Circle circle)
        {
            if (from != null && to != null && circle != null)
            {
                var circleInvitation = new CircleInvitation
                {
                    From = from,
                    To = to,
                    Circle = circle
                };
                await _applicationDbContext.CircleInvitations.AddAsync(circleInvitation);
                await _applicationDbContext.SaveChangesAsync();
                return circleInvitation;
            }
            return null;
        }

        public async Task Delete(int id)
        {
            var circleInvitation = await GetById(id);
            if (circleInvitation != null)
            {
                _applicationDbContext.Remove(circleInvitation);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task<CircleInvitation?> GetById(int circleInvitationId)
        {
            return await _applicationDbContext.CircleInvitations
                .Include(ci => ci.From)
                .Include(ci => ci.To)
                .Include(ci => ci.Circle)
                .FirstOrDefaultAsync(ci => ci.Id == circleInvitationId);


        }
        public async Task<CircleInvitation?> GetByCircleIdAndUserIdAsync(Guid circleId, string userId)
        {
            return await _applicationDbContext.CircleInvitations
                .Include(ci => ci.From)
                .Include(ci => ci.To)
                .Include(ci => ci.Circle)
                .FirstOrDefaultAsync(ci => ci.To.Id == userId && ci.Circle.Id == circleId);
        }
    }
}
