using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IFriendRequest
    {
        Task<FriendRequest> GetById(int id);
        Task UpdateFriendRequestAsync(FriendRequest friendRequest);
        Task CreateFriendRequest(User from, User to);
        Task<FriendRequest> CheckExistingFriendRequestAsync(User user, User otherUser);
        Task<List<FriendRequest>> PendingFriendRequests(string username);
        Task<List<FriendRequest>> OutgoingFriendRequests(string username);
    }
}
