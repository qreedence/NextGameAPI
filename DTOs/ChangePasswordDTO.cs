namespace NextGameAPI.DTOs
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; } = "";
        public required string NewPassword { get; set; }
    }
}
