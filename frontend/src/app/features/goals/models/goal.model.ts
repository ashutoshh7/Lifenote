export type GoalCategory = 'Work' | 'Personal' | 'Health' | 'Finance' | 'Learning' | 'Other';
export type GoalStatus = 'Active' | 'Paused' | 'Completed' | 'Archived';

export interface IMilestone {
  id: string;
  goalId: string;
  title: string;
  description?: string;
  targetDate?: string;
  isCompleted: boolean;
  completedAt?: string;
  createdAt: string;
}

export interface IGoal {
  id: string;
  title: string;
  description?: string;
  category: GoalCategory;
  status: GoalStatus;
  targetDate?: string;
  linkedNoteId?: string;
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
  linkedNoteId?: string;
}

export interface ICreateMilestoneDto {
  goalId: string;
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
  linkedNoteId?: string;
}

export interface IUpdateMilestoneDto {
  title?: string;
  description?: string;
  targetDate?: string;
  isCompleted?: boolean;
}
