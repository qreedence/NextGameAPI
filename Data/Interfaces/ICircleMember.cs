using NextGameAPI.Constants;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircleMember
    {
        Task<CircleMember> CreateCircleMemberAsync(Guid circleId, string username, CircleMemberRole role);
        Task<CircleMember> GetByIdAsync(Guid circleMemberId);
        Task UpdateCircleMemberAsync(CircleMember circleMember);
        Task DeleteCircleMemberAsync(Guid circleMemberId);
    }
}
