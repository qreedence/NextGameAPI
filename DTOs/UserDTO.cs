﻿namespace NextGameAPI.DTOs
{
    public class UserDTO
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public string? Avatar { get; set; }
        public bool AccountIsPublic { get; set; }
    }
}
