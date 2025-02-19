using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IFriendship
    {
        Task<List<User>> GetFriendsForUserAsync(User user);
    }
}
