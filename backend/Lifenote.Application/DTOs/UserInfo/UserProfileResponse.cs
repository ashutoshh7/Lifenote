namespace Lifenote.Application.DTOs.UserInfo
{
    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public string Theme { get; set; } = "light";
        public DateTime? LastLoginAt { get; set; }
    }
}
