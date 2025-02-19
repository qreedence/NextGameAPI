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

        public async Task<List<User>> SearchUsersAsync(string userName)
        {
            List<User> users = await _applicationDbContext.Users
                .Include(x => x.Settings)
                .Where(x => x.Settings.AccountIsPublic == true 
                    && !string.IsNullOrEmpty(x.UserName) 
                    && EF.Functions.Like(x.UserName, $"%{userName}%"))
                .ToListAsync();

            return users;
        }
    }
}
