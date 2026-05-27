using System.Collections.Generic;
using System.Threading.Tasks;
using Lifenote.Application.DTOs.Goal;

namespace Lifenote.Application.Contracts;

public interface IGoalService
{
    Task<IEnumerable<GoalDto>> GetGoalsAsync(Guid userId);
    Task<GoalDto?> GetGoalByIdAsync(Guid id, Guid userId);
    Task<GoalDto> CreateGoalAsync(Guid userId, CreateGoalDto dto);
    Task<GoalDto> UpdateGoalAsync(Guid id, Guid userId, UpdateGoalDto dto);
    Task<bool> DeleteGoalAsync(Guid id, Guid userId);

    Task<MilestoneDto> AddMilestoneAsync(Guid goalId, Guid userId, CreateMilestoneDto dto);
    Task<MilestoneDto> UpdateMilestoneAsync(Guid milestoneId, Guid goalId, Guid userId, UpdateMilestoneDto dto);
    Task<bool> DeleteMilestoneAsync(Guid milestoneId, Guid goalId, Guid userId);
}
