using System.Collections.Generic;
using System.Threading.Tasks;
using Lifenote.Application.DTOs.Goal;

namespace Lifenote.Application.Contracts;

public interface IGoalService
{
    Task<IEnumerable<GoalDto>> GetGoalsAsync(int userId);
    Task<GoalDto?> GetGoalByIdAsync(int id, int userId);
    Task<GoalDto> CreateGoalAsync(int userId, CreateGoalDto dto);
    Task<GoalDto> UpdateGoalAsync(int id, int userId, UpdateGoalDto dto);
    Task<bool> DeleteGoalAsync(int id, int userId);

    Task<MilestoneDto> AddMilestoneAsync(int goalId, int userId, CreateMilestoneDto dto);
    Task<MilestoneDto> UpdateMilestoneAsync(int milestoneId, int goalId, int userId, UpdateMilestoneDto dto);
    Task<bool> DeleteMilestoneAsync(int milestoneId, int goalId, int userId);
}
