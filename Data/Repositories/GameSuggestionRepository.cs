﻿using Microsoft.EntityFrameworkCore;
using NextGameAPI.Data.Interfaces;
using NextGameAPI.Data.Models;

namespace NextGameAPI.Data.Repositories
{
    public class GameSuggestionRepository : IGameSuggestion
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GameSuggestionRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task AddAsync(GameSuggestion gameSuggestion)
        {
            if (gameSuggestion == null)
            {
                return;
            }
            
            await _applicationDbContext.GameSuggestions.AddAsync(gameSuggestion);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<GameSuggestion?> GetByIdAsync(int id)
        {
            if (id == 0)
            {
                return null;
            }

            return await _applicationDbContext.GameSuggestions.Include(gs => gs.Votes).FirstOrDefaultAsync(gs => gs.Id == id);
        }

        public async Task<GameSuggestion?> GetByGameIdAsync(Guid circleId, int gameId)
        {
            if (gameId == 0)
            {
                return null;
            }

            return await _applicationDbContext.GameSuggestions.Include(gs => gs.Votes).FirstOrDefaultAsync(gs => gs.CircleId == circleId && gs.GameId == gameId);
        }

        public async Task<List<GameSuggestion>> GetAllByCircleId(Guid circleId)
        {
            var suggestions = await _applicationDbContext.GameSuggestions.Include(gs => gs.Votes).Where(gs => gs.CircleId == circleId).ToListAsync();
            if (suggestions == null || suggestions.Count == 0)
            {
                return new List<GameSuggestion>();
            }
            return suggestions;
        }

        public async Task UpdateAsync(GameSuggestion gameSuggestion)
        {
            if (gameSuggestion == null)
            {
                return;
            }

            _applicationDbContext.GameSuggestions.Update(gameSuggestion);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var gameSuggestion = await _applicationDbContext.GameSuggestions.FirstOrDefaultAsync(gs => gs.Id == id);
            if (gameSuggestion == null)
            {
                return;
            }

            _applicationDbContext.GameSuggestions.Remove(gameSuggestion);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
