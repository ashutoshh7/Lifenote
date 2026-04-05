using System.ComponentModel.DataAnnotations;

namespace Lifenote.Application.DTOs.Habit
{
    public class UpdateHabitDto
    {
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format")]
        public string? Color { get; set; }

        public string? IconName { get; set; }

        [RegularExpression("^(Daily|Weekly|Custom)$", ErrorMessage = "Frequency must be Daily, Weekly, or Custom")]
        public string? FrequencyType { get; set; }

        public string? FrequencyValue { get; set; }

        [Range(1, 10, ErrorMessage = "Target count must be between 1 and 10")]
        public int? TargetCount { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
