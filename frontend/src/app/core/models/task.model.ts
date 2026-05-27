import { BaseEntity } from "./base-entity.model";

// Why enums? They provide type safety and prevent magic strings
export enum TaskPriority {
  LOW = 'low',
  MEDIUM = 'medium', 
  HIGH = 'high'
}

export enum TaskStatus {
  PENDING = 'pending',
  IN_PROGRESS = 'in_progress',
  COMPLETED = 'completed',
  CANCELLED = 'cancelled'
}

// Main Task interface extending BaseEntity
export interface Task extends BaseEntity {
  title: string;
  description?: string; // Optional with ?
  priority: TaskPriority;
  status: TaskStatus;
  dueDate?: Date;
  estimatedPomodoros?: number; // Integration with Pomodoro feature
  tags: string[]; // Array for categorization
  subtasks: SubTask[];
}

// Nested interface for subtasks
export interface SubTask {
  id: string;
  title: string;
  completed: boolean;
}

// DTO (Data Transfer Object) for creating new tasks
export interface CreateTaskDto {
  title: string;
  description?: string;
  priority: TaskPriority;
  dueDate?: Date;
  estimatedPomodoros?: number;
  tags?: string[];
}
