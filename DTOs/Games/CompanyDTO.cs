﻿using NextGameAPI.Constants;
using System.Text.Json.Serialization;

namespace NextGameAPI.DTOs.Games
{
    public class CompanyDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
