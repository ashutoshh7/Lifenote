using System;
using System.Collections.Generic;

namespace Lifenote.Application.DTOs.Goal;

public class CreateGoalDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? TargetDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Category { get; set; }
    public List<CreateMilestoneDto> Milestones { get; set; } = new();
}
