using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircle
    {
        Task CreateCircleAsync(Circle circle);
        Task<Circle> GetByIdAsync(Guid id);
        Task UpdateCircleAsync(Circle circle);
        Task DeleteCircleByIdAsync(Guid id);
        Task AddUserToCircleAsync(Guid circleId, Guid circleMemberId);
        Task RemoveUserFromCircleAsync(Guid circleId, Guid circleMemberId);
    }
}
