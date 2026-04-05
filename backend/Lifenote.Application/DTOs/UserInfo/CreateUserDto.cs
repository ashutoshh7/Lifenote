namespace Lifenote.Application.DTOs.UserInfo
{
    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Bio { get; set; } = string.Empty;
    }

    public class UpdateThemeDto
    {
        public string Theme { get; set; } = string.Empty; // "light", "dark", "auto"
    }
}
