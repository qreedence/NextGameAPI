using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface ICircleGame
    {
        Task AddAsync(CircleGame circleGame);
        Task UpdateAsync(CircleGame circleGame);
        Task<CircleGame?> GetByIdAsync(int id);
    }
}
