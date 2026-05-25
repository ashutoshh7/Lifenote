export type GoalCategory = 'Work' | 'Personal' | 'Health' | 'Finance' | 'Learning' | 'Other';
export type GoalStatus = 'Active' | 'Paused' | 'Completed' | 'Archived';

export interface IMilestone {
  id: number;
  goalId: number;
  title: string;
  description?: string;
  targetDate?: string;
  isCompleted: boolean;
  completedAt?: string;
  createdAt: string;
}

export interface IGoal {
  id: number;
  title: string;
  description?: string;
  category: GoalCategory;
  status: GoalStatus;
  targetDate?: string;
  linkedNoteId?: number;
  milestones: IMilestone[];
  createdAt: string;
  updatedAt?: string;
}

export interface ICreateGoalDto {
  title: string;
  description?: string;
  category: GoalCategory;
  status?: GoalStatus;
  targetDate?: string;
  linkedNoteId?: number;
}

export interface ICreateMilestoneDto {
  goalId: number;
  title: string;
  description?: string;
  targetDate?: string;
}

export interface IUpdateGoalDto {
  title?: string;
  description?: string;
  category?: GoalCategory;
  status?: GoalStatus;
  targetDate?: string;
  linkedNoteId?: number;
}

export interface IUpdateMilestoneDto {
  title?: string;
  description?: string;
  targetDate?: string;
  isCompleted?: boolean;
}
