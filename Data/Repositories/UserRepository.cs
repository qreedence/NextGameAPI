using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
           if (!string.IsNullOrEmpty(username))
            {
                return await _applicationDbContext.Users
                    .Include(u => u.Settings)
                    .FirstOrDefaultAsync(u => u.UserName == username);
            }
            return null;
        }

        public async Task<List<User>> SearchUsersAsync(string userName)
        {
            List<User> users = await _applicationDbContext.Users
                .Include(x => x.Settings)
                .Where(x => x.Settings.AccountIsPublic == true
                    && !string.IsNullOrEmpty(x.UserName)
                    && x.UserName.StartsWith(userName))
                .ToListAsync();

            return users;
        }

        public async Task<List<User>> GetEligibleFriendsForCircleAsync(
            List<string> friendIds,
            List<string> existingMemberIds,
            string usernameFilter)
        {
            usernameFilter = usernameFilter?.Trim() ?? "";

            return await _applicationDbContext.Users
                .Where(u =>
                    friendIds.Contains(u.Id) &&
                    !existingMemberIds.Contains(u.Id) &&
                    (string.IsNullOrEmpty(usernameFilter) || u.UserName.StartsWith(usernameFilter)))
                .ToListAsync();
        }
    }
}
