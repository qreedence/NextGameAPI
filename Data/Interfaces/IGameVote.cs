using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IGameVote
    {
        Task<GameVote?> GetById (int id);
        Task Add(GameVote gameVote);
        Task Update(GameVote gameVote);
    }
}
