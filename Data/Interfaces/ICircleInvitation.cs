using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircleInvitation
    {
        Task<CircleInvitation> Create(User from, User to, Circle circle);
        Task<CircleInvitation?> CheckExisting(User to, Guid circleId);
        Task<CircleInvitation?> GetById(int circleInvitationId);
        Task<CircleInvitation?> GetByCircleIdAndUserIdAsync(Guid circleId, string userId);
        Task Delete(int id);
    }
}
