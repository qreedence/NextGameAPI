﻿using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class GameSearchResultDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("cover")]
        public GameCoverDTO Cover { get; set; }
        public string CoverUrl { get; set; }
        [JsonPropertyName("first_release_date")]
        public long? FirstReleaseDateUnix { get; set; }
        public DateTime FirstReleaseDate { get; set; }
        [JsonPropertyName("aggregated_rating")]
        public double AggregatedRating { get; set; }

        [JsonPropertyName("aggregated_rating_count")]
        public int AggregatedRatingCount { get; set; }
    }
}
