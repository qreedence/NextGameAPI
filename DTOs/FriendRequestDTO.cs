namespace NextGameAPI.DTOs
{
    public class FriendRequestDTO
    {
        public string From { get; set; }
        public string To { get; set; }
        public DateTime SentAt { get; set; }
    }
}
