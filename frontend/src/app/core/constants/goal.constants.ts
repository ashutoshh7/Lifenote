import { GoalCategory, GoalStatus } from '../../features/goals/models/goal.model';

export type GoalSortOption = 'newest' | 'oldest' | 'due-soon' | 'progress-asc' | 'progress-desc' | 'a-z';

export const GOAL_CATEGORIES: GoalCategory[] = [
  'Work', 'Personal', 'Health', 'Finance', 'Learning', 'Other'
];

export const GOAL_STATUSES: GoalStatus[] = [
  'Active', 'Paused', 'Completed', 'Archived'
];

export const GOAL_CATEGORY_COLORS: Record<string, string> = {
  Work: '#3d5afe', 
  Personal: '#7c3aed', 
  Health: '#53e076',
  Finance: '#f9c94c', 
  Learning: '#06b6d4', 
  Other: '#9ca3af'
};

export const GOAL_SORT_OPTIONS: { value: GoalSortOption; label: string }[] = [
  { value: 'newest',       label: 'Newest First' },
  { value: 'oldest',       label: 'Oldest First' },
  { value: 'due-soon',     label: 'Due Soon' },
  { value: 'progress-asc', label: 'Progress ↑' },
  { value: 'progress-desc',label: 'Progress ↓' },
  { value: 'a-z',          label: 'A – Z' }
];
