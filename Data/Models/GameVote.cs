﻿using NextGameAPI.Constants;

namespace NextGameAPI.Data.Models
{
    public class GameVote
    {
        public int Id { get; set; }
        public User User { get; set; }
        public GameVoteStatus Status { get; set; }
    }
}
