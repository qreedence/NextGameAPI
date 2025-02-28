using Microsoft.EntityFrameworkCore;
using NextGameAPI.Constants;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class CircleMemberRepository : ICircleMember
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICircle _circleRepository;
        private readonly IUser _userRepository;

        public CircleMemberRepository(ApplicationDbContext applicationDbContext, ICircle circleRepository, IUser userRepository)
        {
            _applicationDbContext = applicationDbContext;
            _circleRepository = circleRepository;
            _userRepository = userRepository;
        }

        public async Task<CircleMember> CreateCircleMemberAsync(Guid circleId, string userId, CircleMemberRole role)
        {
            var circle = await _circleRepository.GetByIdAsync(circleId);
            var user = await _userRepository.FindByUsernameAsync(userId);
            if (circle != null && user != null)
            {
                var circleMember = new CircleMember
                {
                    Circle = circle,
                    User = user,
                    Role = role,
                    IsActive = true,
                };
                await _applicationDbContext.CircleMembers.AddAsync(circleMember);
                await _applicationDbContext.SaveChangesAsync();
                return circleMember;
            }
            return null;
        }

        public async Task DeleteCircleMemberAsync(Guid circleMemberId)
        {
            var circleMember = await GetByIdAsync(circleMemberId);
            if (circleMember != null)
            {
               _applicationDbContext.CircleMembers.Remove(circleMember);
                await _applicationDbContext.SaveChangesAsync();
            }
        }

        public async Task<CircleMember> GetByIdAsync(Guid circleMemberId)
        {
            if (circleMemberId != Guid.Empty)
            {
                var circleMember = await _applicationDbContext.CircleMembers.FirstOrDefaultAsync(cm => cm.Id == circleMemberId);
                if (circleMember != null)
                {
                    return circleMember;
                }
            }
            return null;
        }

        public async Task UpdateCircleMemberAsync(CircleMember circleMember)
        {
            if (circleMember != null)
            {
                _applicationDbContext.CircleMembers.Update(circleMember);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
    }
}
