using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircleMember
    {
        Task CreateCircleMemberAsync(Guid circleId, string userId);
        Task<CircleMember> GetByIdAsync(Guid circleMemberId);
        Task UpdateCircleMemberAsync(Guid circleMemberId);
        Task DeleteCircleMemberAsync(Guid circleMemberId);
    }
}
