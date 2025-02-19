using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class FriendshipRepository : IFriendship
    {

        private readonly ApplicationDbContext _applicationDbContext;

        public FriendshipRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<User>> GetFriendsForUserAsync(User user)
        {
            if (user == null)
            {
                return null;
            }

            var friendships = await _applicationDbContext.Friendships
                .Where(friendship => (friendship.UserA == user) || (friendship.UserB == user))
                .Include(x => x.UserB.Settings)
                .Include(x => x.UserA.Settings)
                .ToListAsync();

            List<User> friends = friendships.Select(friendship => {
                return friendship.UserA == user ? friendship.UserB : friendship.UserA;
            }).Distinct().ToList();

            return friends;
        }
    }
}
