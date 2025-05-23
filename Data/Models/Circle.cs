﻿namespace NextGameAPI.Data.Models
{
    public class Circle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CircleMember> CircleMembers { get; set; } = [];
        public List<GameSuggestion> SuggestionQueue { get; set; } = [];
        public List<CircleGame> CircleGames { get; set; } = [];
    }
}
