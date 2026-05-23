using System.ComponentModel.DataAnnotations;

namespace Lifenote.Application.DTOs.Habit
{
    public class CheckInDto
    {
        [Required(ErrorMessage = "Habit ID is required")]
        public int HabitId { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }
}
