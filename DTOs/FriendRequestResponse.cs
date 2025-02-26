using NextGameAPI.Data.Models;

namespace NextGameAPI.DTOs
{
    public class FriendRequestResponse
    {
        public int Id { get; set; }
        public FriendRequestStatus Status { get; set; }
    }
}
