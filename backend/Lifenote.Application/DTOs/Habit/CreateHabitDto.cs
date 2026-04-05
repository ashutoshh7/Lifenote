using System.ComponentModel.DataAnnotations;

namespace Lifenote.Application.DTOs.Habit
{
    public class CreateHabitDto
    {
        [Required(ErrorMessage = "Habit name is required")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid color format. Use hex format like #4CAF50")]
        public string Color { get; set; } = "#4CAF50";

        [Required(ErrorMessage = "Icon name is required")]
        public string IconName { get; set; } = "fa-check";

        [Required(ErrorMessage = "Frequency type is required")]
        [RegularExpression("^(Daily|Weekly|Custom)$", ErrorMessage = "Frequency must be Daily, Weekly, or Custom")]
        public string FrequencyType { get; set; } = "Daily";

        public string? FrequencyValue { get; set; }

        [Range(1, 10, ErrorMessage = "Target count must be between 1 and 10")]
        public int TargetCount { get; set; } = 1;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
