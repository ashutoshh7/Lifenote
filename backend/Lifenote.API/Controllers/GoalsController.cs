using System.Collections.Generic;
using System.Threading.Tasks;
using Lifenote.API.Models.Responses;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Goal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lifenote.API.Controllers;

[Route("api/[controller]")]
public class GoalsController : ApiControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService, ICurrentUserService currentUserService)
        : base(currentUserService)
    {
        _goalService = goalService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<GoalDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<GoalDto>>>> GetGoals()
    {
        var userId = await GetUserIdAsync();
        var goals = await _goalService.GetGoalsAsync(userId);
        return Ok(ApiResponse<IEnumerable<GoalDto>>.Success(goals));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GoalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<GoalDto>>> GetGoalById(int id)
    {
        var userId = await GetUserIdAsync();
        var goal = await _goalService.GetGoalByIdAsync(id, userId);
        if (goal == null)
            return NotFound(ApiResponse<object>.Fail("Goal not found."));

        return Ok(ApiResponse<GoalDto>.Success(goal));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<GoalDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<GoalDto>>> CreateGoal([FromBody] CreateGoalDto dto)
    {
        var userId = await GetUserIdAsync();
        var goal = await _goalService.CreateGoalAsync(userId, dto);
        return CreatedAtAction(
            nameof(GetGoalById),
            new { id = goal.Id },
            ApiResponse<GoalDto>.Success(goal, "Goal created successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GoalDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<GoalDto>>> UpdateGoal(int id, [FromBody] UpdateGoalDto dto)
    {
        var userId = await GetUserIdAsync();
        var goal = await _goalService.UpdateGoalAsync(id, userId, dto);
        return Ok(ApiResponse<GoalDto>.Success(goal, "Goal updated successfully."));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteGoal(int id)
    {
        var userId = await GetUserIdAsync();
        var succeeded = await _goalService.DeleteGoalAsync(id, userId);
        if (!succeeded)
            return NotFound(ApiResponse<object>.Fail("Goal not found."));

        return Ok(ApiResponse<object>.Success(new { }, "Goal deleted successfully."));
    }

    [HttpPost("{goalId}/milestones")]
    [ProducesResponseType(typeof(ApiResponse<MilestoneDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult<ApiResponse<MilestoneDto>>> AddMilestone(int goalId, [FromBody] CreateMilestoneDto dto)
    {
        var userId = await GetUserIdAsync();
        var milestone = await _goalService.AddMilestoneAsync(goalId, userId, dto);
        return CreatedAtAction(
            nameof(GetGoalById),
            new { id = goalId },
            ApiResponse<MilestoneDto>.Success(milestone, "Milestone added successfully."));
    }

    [HttpPut("{goalId}/milestones/{milestoneId}")]
    [ProducesResponseType(typeof(ApiResponse<MilestoneDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<MilestoneDto>>> UpdateMilestone(int goalId, int milestoneId, [FromBody] UpdateMilestoneDto dto)
    {
        var userId = await GetUserIdAsync();
        var milestone = await _goalService.UpdateMilestoneAsync(milestoneId, goalId, userId, dto);
        return Ok(ApiResponse<MilestoneDto>.Success(milestone, "Milestone updated successfully."));
    }

    [HttpDelete("{goalId}/milestones/{milestoneId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMilestone(int goalId, int milestoneId)
    {
        var userId = await GetUserIdAsync();
        var succeeded = await _goalService.DeleteMilestoneAsync(milestoneId, goalId, userId);
        if (!succeeded)
            return NotFound(ApiResponse<object>.Fail("Milestone not found."));

        return Ok(ApiResponse<object>.Success(new { }, "Milestone deleted successfully."));
    }
}
