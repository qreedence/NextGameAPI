using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class CircleMemberRepository : ICircleMember
    {
        public Task CreateCircleMemberAsync(Guid circleId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCircleMemberAsync(Guid circleMemberId)
        {
            throw new NotImplementedException();
        }

        public Task<CircleMember> GetByIdAsync(Guid circleMemberId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCircleMemberAsync(Guid circleMemberId)
        {
            throw new NotImplementedException();
        }
    }
}
