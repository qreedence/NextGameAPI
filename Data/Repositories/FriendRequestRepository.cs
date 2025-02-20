using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class FriendRequestRepository : IFriendRequest
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FriendRequestRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task CreateFriendRequest(User from, User to)
        {
            if (from != null && to != null)
            {
                var checkExisting = _applicationDbContext.FriendRequests.FirstOrDefault(fr => fr.From == from && fr.To == to);
                if (checkExisting == null)
                {
                    await _applicationDbContext.FriendRequests.AddAsync(new FriendRequest { From = from, To = to});
                    await _applicationDbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<List<FriendRequest>> OutgoingFriendRequests(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new List<FriendRequest>();
            }
            return await _applicationDbContext.FriendRequests
                .Include(x => x.From)
                .Include(x => x.To)
                .Where(fr => fr.From.UserName == username)
                .ToListAsync() ;
        }

        public async Task<List<FriendRequest>> PendingFriendRequests(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new List<FriendRequest>();
            }
            return await _applicationDbContext.FriendRequests
                .Include(x => x.From)
                .Include(x => x.To)
                .Where(fr => fr.To.UserName == username)
                .ToListAsync();
        }
    }
}
