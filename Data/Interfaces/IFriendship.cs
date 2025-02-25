using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IFriendship
    {
        Task CreateFriendship(Friendship friendship);
        Task<List<User>> GetFriendsForUserAsync(User user);
    }
}
