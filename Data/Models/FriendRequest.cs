namespace NextGameAPI.Data.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public User From { get; set; }
        public User To { get; set; }
        public DateTime SentAt { get; set; }
    }
}
