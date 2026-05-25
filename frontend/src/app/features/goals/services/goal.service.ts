import { Injectable, signal, computed } from '@angular/core';
import { IGoal, IMilestone, ICreateGoalDto, IUpdateGoalDto, ICreateMilestoneDto, IUpdateMilestoneDto } from '../models/goal.model';

@Injectable({ providedIn: 'root' })
export class GoalService {
  // Local state (replace with HTTP calls when backend is ready)
  goals = signal<IGoal[]>(this.getMockGoals());
  isLoading = signal(false);

  private nextId = 10;

  // Computed
  activeGoals = computed(() => this.goals().filter(g => g.status === 'Active'));
  completedGoals = computed(() => this.goals().filter(g => g.status === 'Completed'));

  getGoalById(id: number): IGoal | undefined {
    return this.goals().find(g => g.id === id);
  }

  getProgress(goal: IGoal): number {
    if (!goal.milestones?.length) return 0;
    const completed = goal.milestones.filter(m => m.isCompleted).length;
    return Math.round((completed / goal.milestones.length) * 100);
  }

  getDaysLeft(goal: IGoal): number | null {
    if (!goal.targetDate) return null;
    const target = new Date(goal.targetDate);
    const today = new Date();
    const diff = Math.ceil((target.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
    return diff;
  }

  createGoal(dto: ICreateGoalDto): IGoal {
    const newGoal: IGoal = {
      id: this.nextId++,
      title: dto.title,
      description: dto.description,
      category: dto.category,
      status: dto.status ?? 'Active',
      targetDate: dto.targetDate,
      linkedNoteId: dto.linkedNoteId,
      milestones: [],
      createdAt: new Date().toISOString(),
    };
    this.goals.update(gs => [newGoal, ...gs]);
    return newGoal;
  }

  updateGoal(id: number, dto: IUpdateGoalDto): void {
    this.goals.update(gs => gs.map(g =>
      g.id === id ? { ...g, ...dto, updatedAt: new Date().toISOString() } : g
    ));
  }

  deleteGoal(id: number): void {
    this.goals.update(gs => gs.filter(g => g.id !== id));
  }

  addMilestone(dto: ICreateMilestoneDto): IMilestone {
    const milestone: IMilestone = {
      id: this.nextId++,
      goalId: dto.goalId,
      title: dto.title,
      description: dto.description,
      targetDate: dto.targetDate,
      isCompleted: false,
      createdAt: new Date().toISOString(),
    };
    this.goals.update(gs => gs.map(g =>
      g.id === dto.goalId ? { ...g, milestones: [...g.milestones, milestone] } : g
    ));
    return milestone;
  }

  updateMilestone(goalId: number, milestoneId: number, dto: IUpdateMilestoneDto): void {
    this.goals.update(gs => gs.map(g =>
      g.id === goalId
        ? {
            ...g,
            milestones: g.milestones.map(m =>
              m.id === milestoneId
                ? { ...m, ...dto, completedAt: dto.isCompleted ? new Date().toISOString() : undefined }
                : m
            )
          }
        : g
    ));
  }

  toggleMilestone(goalId: number, milestoneId: number): void {
    const goal = this.getGoalById(goalId);
    const milestone = goal?.milestones.find(m => m.id === milestoneId);
    if (milestone) {
      this.updateMilestone(goalId, milestoneId, { isCompleted: !milestone.isCompleted });
    }
  }

  deleteMilestone(goalId: number, milestoneId: number): void {
    this.goals.update(gs => gs.map(g =>
      g.id === goalId
        ? { ...g, milestones: g.milestones.filter(m => m.id !== milestoneId) }
        : g
    ));
  }

  private getMockGoals(): IGoal[] {
    return [
      {
        id: 1,
        title: 'Launch Design System',
        description: 'Build a comprehensive design system for all products.',
        category: 'Work',
        status: 'Active',
        targetDate: new Date(Date.now() + 45 * 86400000).toISOString(),
        milestones: [
          { id: 1, goalId: 1, title: 'Finalize color tokens', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 2, goalId: 1, title: 'Build component library', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 3, goalId: 1, title: 'Write documentation', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 4, goalId: 1, title: 'Ship v1.0 release', isCompleted: false, createdAt: new Date().toISOString() },
        ],
        createdAt: new Date().toISOString(),
      },
      {
        id: 2,
        title: 'Read 12 Books This Year',
        description: 'Read one book per month across different genres.',
        category: 'Personal',
        status: 'Active',
        targetDate: new Date('2024-12-31').toISOString(),
        milestones: [
          { id: 5, goalId: 2, title: 'Deep Work', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 6, goalId: 2, title: 'Atomic Habits', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 7, goalId: 2, title: 'The Lean Startup', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 8, goalId: 2, title: 'Zero to One', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 9, goalId: 2, title: 'Thinking Fast and Slow', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 10, goalId: 2, title: 'The Psychology of Money', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 11, goalId: 2, title: 'Sapiens', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 12, goalId: 2, title: 'The Pragmatic Programmer', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 13, goalId: 2, title: 'Clean Code', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 14, goalId: 2, title: 'The Hard Thing About Hard Things', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 15, goalId: 2, title: 'Rework', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 16, goalId: 2, title: 'Inspired', isCompleted: false, createdAt: new Date().toISOString() },
        ],
        createdAt: new Date().toISOString(),
      },
      {
        id: 3,
        title: 'Run a Half Marathon',
        description: 'Train and complete a half marathon by end of year.',
        category: 'Health',
        status: 'Active',
        targetDate: new Date('2024-11-15').toISOString(),
        milestones: [
          { id: 17, goalId: 3, title: 'Run 5km without stopping', isCompleted: true, createdAt: new Date().toISOString() },
          { id: 18, goalId: 3, title: 'Complete 10km run', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 19, goalId: 3, title: 'Run 15km training', isCompleted: false, createdAt: new Date().toISOString() },
          { id: 20, goalId: 3, title: 'Race day', isCompleted: false, createdAt: new Date().toISOString() },
        ],
        createdAt: new Date().toISOString(),
      },
    ];
  }
}
