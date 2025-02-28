namespace NextGameAPI.Data.Models
{
    public class Circle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public required User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<CircleMember> CircleMembers { get; set; }

        //TODO: Implement game related stuff
    }
}
