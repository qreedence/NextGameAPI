using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IGameVote
    {
        Task<GameVote?> GetByIdAsync (int id);
        Task AddAsync(GameVote gameVote);
        Task UpdateAsync(GameVote gameVote);
    }
}
