using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lifenote.Application.Contracts;
using Lifenote.Application.DTOs.Goal;
using Lifenote.Domain.Entities;
using Lifenote.Domain.Exceptions;
using Lifenote.Domain.Interfaces;

namespace Lifenote.Application.Services;

public class GoalService : IGoalService
{
    private readonly IUnitOfWork _unitOfWork;

    public GoalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<GoalDto>> GetGoalsAsync(int userId)
    {
        var goals = await _unitOfWork.Goals.GetByUserIdAsync(userId);
        return goals.Select(MapToGoalDto);
    }

    public async Task<GoalDto?> GetGoalByIdAsync(int id, int userId)
    {
        var goal = await _unitOfWork.Goals.GetByIdAsync(id);
        if (goal == null || goal.UserId != userId) return null;
        return MapToGoalDto(goal);
    }

    public async Task<GoalDto> CreateGoalAsync(int userId, CreateGoalDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new DomainException("Goal title cannot be empty.");

        var goal = new Goal
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            TargetDate = dto.TargetDate,
            Status = dto.Status,
            Category = dto.Category,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (dto.Milestones != null && dto.Milestones.Any())
        {
            foreach (var mDto in dto.Milestones)
            {
                if (string.IsNullOrWhiteSpace(mDto.Title))
                    throw new DomainException("Milestone title cannot be empty.");

                goal.Milestones.Add(new Milestone
                {
                    Title = mDto.Title,
                    Description = mDto.Description,
                    TargetDate = mDto.TargetDate,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        await _unitOfWork.Goals.AddAsync(goal);
        await _unitOfWork.SaveChangesAsync();

        return MapToGoalDto(goal);
    }

    public async Task<GoalDto> UpdateGoalAsync(int id, int userId, UpdateGoalDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new DomainException("Goal title cannot be empty.");

        var goal = await _unitOfWork.Goals.GetByIdAsync(id)
            ?? throw new NotFoundException($"Goal {id} not found.");

        if (goal.UserId != userId)
            throw new NotFoundException($"Goal {id} not found.");

        goal.Title = dto.Title;
        goal.Description = dto.Description;
        goal.TargetDate = dto.TargetDate;
        goal.Status = dto.Status;
        goal.Category = dto.Category;
        goal.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Goals.Update(goal);
        await _unitOfWork.SaveChangesAsync();

        return MapToGoalDto(goal);
    }

    public async Task<bool> DeleteGoalAsync(int id, int userId)
    {
        var goal = await _unitOfWork.Goals.GetByIdAsync(id);
        if (goal == null || goal.UserId != userId) return false;

        _unitOfWork.Goals.Remove(goal);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<MilestoneDto> AddMilestoneAsync(int goalId, int userId, CreateMilestoneDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new DomainException("Milestone title cannot be empty.");

        var goal = await _unitOfWork.Goals.GetByIdAsync(goalId)
            ?? throw new NotFoundException($"Goal {goalId} not found.");

        if (goal.UserId != userId)
            throw new NotFoundException($"Goal {goalId} not found.");

        var milestone = new Milestone
        {
            GoalId = goalId,
            Title = dto.Title,
            Description = dto.Description,
            TargetDate = dto.TargetDate,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        goal.Milestones.Add(milestone);
        goal.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Goals.Update(goal);
        await _unitOfWork.SaveChangesAsync();

        return MapToMilestoneDto(milestone);
    }

    public async Task<MilestoneDto> UpdateMilestoneAsync(int milestoneId, int goalId, int userId, UpdateMilestoneDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new DomainException("Milestone title cannot be empty.");

        var goal = await _unitOfWork.Goals.GetByIdAsync(goalId)
            ?? throw new NotFoundException($"Goal {goalId} not found.");

        if (goal.UserId != userId)
            throw new NotFoundException($"Goal {goalId} not found.");

        var milestone = goal.Milestones.FirstOrDefault(m => m.Id == milestoneId)
            ?? throw new NotFoundException($"Milestone {milestoneId} not found in Goal {goalId}.");

        milestone.Title = dto.Title;
        milestone.Description = dto.Description;
        milestone.TargetDate = dto.TargetDate;
        
        if (milestone.IsCompleted != dto.IsCompleted)
        {
            milestone.IsCompleted = dto.IsCompleted;
            milestone.CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null;
        }

        milestone.UpdatedAt = DateTime.UtcNow;
        goal.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Goals.Update(goal);
        await _unitOfWork.SaveChangesAsync();

        return MapToMilestoneDto(milestone);
    }

    public async Task<bool> DeleteMilestoneAsync(int milestoneId, int goalId, int userId)
    {
        var goal = await _unitOfWork.Goals.GetByIdAsync(goalId);
        if (goal == null || goal.UserId != userId) return false;

        var milestone = goal.Milestones.FirstOrDefault(m => m.Id == milestoneId);
        if (milestone == null) return false;

        goal.Milestones.Remove(milestone);
        goal.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Goals.Update(goal);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static GoalDto MapToGoalDto(Goal goal) => new()
    {
        Id = goal.Id,
        UserId = goal.UserId,
        Title = goal.Title,
        Description = goal.Description,
        TargetDate = goal.TargetDate,
        Status = goal.Status,
        Category = goal.Category,
        Milestones = goal.Milestones?.Select(MapToMilestoneDto).ToList() ?? new List<MilestoneDto>(),
        CreatedAt = goal.CreatedAt,
        UpdatedAt = goal.UpdatedAt
    };

    private static MilestoneDto MapToMilestoneDto(Milestone milestone) => new()
    {
        Id = milestone.Id,
        GoalId = milestone.GoalId,
        Title = milestone.Title,
        Description = milestone.Description,
        TargetDate = milestone.TargetDate,
        IsCompleted = milestone.IsCompleted,
        CompletedAt = milestone.CompletedAt,
        CreatedAt = milestone.CreatedAt,
        UpdatedAt = milestone.UpdatedAt
    };
}
