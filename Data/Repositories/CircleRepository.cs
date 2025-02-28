using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class CircleRepository : ICircle
    {
        public Task AddUserToCircleAsync(Guid circleId, Guid circleMemberId)
        {
            throw new NotImplementedException();
        }

        public Task CreateCircleAsync(Circle circle)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCircleByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Circle> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromCircleAsync(Guid circleId, Guid circleMemberId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCircleAsync(Circle circle)
        {
            throw new NotImplementedException();
        }
    }
}
