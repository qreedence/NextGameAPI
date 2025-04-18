using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Interfaces
{
    public interface IGameSuggestion
    {
        Task AddAsync (GameSuggestion gameSuggestion);
        Task<GameSuggestion?> GetByIdAsync(int id);
        Task<GameSuggestion?> GetByGameIdAsync(Guid circleId, int gameId);
        Task UpdateAsync (GameSuggestion gameSuggestion);
        Task DeleteAsync(int id);
    }
}
