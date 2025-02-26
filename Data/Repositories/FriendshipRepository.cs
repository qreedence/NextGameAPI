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

        public async Task<(bool, int)> CheckExistingFriendshipAsync(User userA, User userB)
        {
            if (userA == null || userB == null)
            {
                return (false, 0);
            }
            var request = await _applicationDbContext.Friendships
                .Include(x => x.UserA)
                .Include(x => x.UserB)
                .FirstOrDefaultAsync(fr =>
                    (fr.UserA.Id == userA.Id && fr.UserB.Id == userB.Id) ||
                    (fr.UserA.Id == userB.Id && fr.UserB.Id == userA.Id));
            if (request != null)
            {
                return (true, request.Id);
            }
            return (false, 0);
        }

        public async Task CreateFriendshipAsync(Friendship friendship)
        {
            if (friendship != null)
            {
                await _applicationDbContext.Friendships.AddAsync(friendship);
                await _applicationDbContext.SaveChangesAsync();
            }
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
