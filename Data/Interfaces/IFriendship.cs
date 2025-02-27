using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IFriendship
    {
        Task<(bool, int)> CheckExistingFriendshipAsync(User userA, User userB);
        Task CreateFriendshipAsync(Friendship friendship);
        Task Unfriend(User user, User friendToRemove);
        Task<List<User>> GetFriendsForUserAsync(User user);
    }
}
