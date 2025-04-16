using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class GameVoteRepository : IGameVote
    {
        public Task Add(GameVote gameVote)
        {
            throw new NotImplementedException();
        }

        public Task<GameVote?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Update(GameVote gameVote)
        {
            throw new NotImplementedException();
        }
    }
}
