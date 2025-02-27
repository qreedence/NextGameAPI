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

        public async Task<FriendRequest> CheckExistingFriendRequestAsync(User userA, User userB)
        {
            if (userA != null && userB != null)
            {
                var request = await _applicationDbContext.FriendRequests
                    .Where(fr =>
                        ((fr.From.Id == userA.Id && fr.To.Id == userB.Id) ||
                        (fr.From.Id == userB.Id && fr.To.Id == userA.Id))
                        && fr.Status == FriendRequestStatus.Pending)
                    .FirstOrDefaultAsync();
                if (request != null)
                {
                    return request;
                }
            }
            return null;
        }
            
        public async Task<bool> CreateFriendRequest(User from, User to)
        {
            if (from != null && to != null)
            {
                var checkExisting = await _applicationDbContext.FriendRequests
                    .Include(fr => fr.From)
                    .Include(fr => fr.To)
                    .FirstOrDefaultAsync(fr => fr.From.Id == from.Id && fr.To.Id == to.Id && fr.Status == FriendRequestStatus.Pending);
                if (checkExisting == null)
                {
                    await _applicationDbContext.FriendRequests.AddAsync(new FriendRequest { From = from, To = to, Status = FriendRequestStatus.Pending});
                    await _applicationDbContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<FriendRequest> GetById(int id)
        {
            var result = await _applicationDbContext.FriendRequests
                .Include(x => x.From)
                .Include(x => x.To)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (result != null)
            {
                return result;
            }
            return null;
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
                .Where(fr => fr.From.UserName == username && fr.Status == FriendRequestStatus.Pending)
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

        public async Task UpdateFriendRequestAsync(FriendRequest friendRequest)
        {
            _applicationDbContext.FriendRequests.Update(friendRequest);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
