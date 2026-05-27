import { BaseEntity } from "./base-entity.model";

export enum PomodoroType {
  FOCUS = 'focus',       // 25 minutes
  SHORT_BREAK = 'short_break', // 5 minutes  
  LONG_BREAK = 'long_break'    // 15 minutes
}

export enum SessionStatus {
  IDLE = 'idle',
  RUNNING = 'running', 
  PAUSED = 'paused',
  COMPLETED = 'completed'
}

export interface PomodoroSession extends BaseEntity {
  type: PomodoroType;
  duration: number; // in seconds
  remainingTime: number;
  status: SessionStatus;
  taskId?: string; // Optional link to a task
  completedAt?: Date;
}

// Configuration interface - allows customization
export interface PomodoroSettings {
  focusDuration: number;    // default 25 minutes
  shortBreakDuration: number; // default 5 minutes
  longBreakDuration: number;  // default 15 minutes
  sessionsUntilLongBreak: number; // default 4
  autoStartBreaks: boolean;
  autoStartPomodoros: boolean;
  soundEnabled: boolean;
}

// Daily statistics
export interface PomodoroStats {
  date: string; // YYYY-MM-DD format
  completedSessions: number;
  totalFocusTime: number; // in minutes
  completedTasks: string[]; // task IDs
}
