using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IFriendRequest
    {
        Task CreateFriendRequest(User from, User to);
        Task<List<FriendRequest>> PendingFriendRequests(string username);
        Task<List<FriendRequest>> OutgoingFriendRequests(string username);
    }
}
