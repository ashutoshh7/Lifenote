using System;

namespace Lifenote.Application.DTOs.Goal;

public class UpdateGoalDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Category { get; set; }
}
