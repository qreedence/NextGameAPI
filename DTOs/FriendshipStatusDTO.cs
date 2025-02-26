namespace NextGameAPI.DTOs
{
    public class FriendshipStatusDTO
    {
        public FriendshipStatus Status { get; set; }
        public int? FriendRequestId { get; set; }
        public int? FriendshipId { get; set; }
    }

    public enum FriendshipStatus
    {
        None = 0,
        Friends = 1,
        OutgoingFriendRequest = 2,
        IncomingFriendRequest = 3,
    }
}
