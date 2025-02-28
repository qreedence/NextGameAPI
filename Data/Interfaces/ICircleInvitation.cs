using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircleInvitation
    {
        Task<CircleInvitation> Create(User from, User to, Circle circle);
        Task<CircleInvitation> GetById(int circleInvitationId);
        Task Delete(int id);
    }
}
