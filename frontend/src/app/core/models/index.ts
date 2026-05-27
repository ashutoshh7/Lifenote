// Barrel export for clean imports
export * from './base-entity.model';
export * from './task.model';
export * from './pomodoro.model';  


// Common types used across the app
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> extends ApiResponse<T[]> {
  totalCount: number;
  pageSize: number;
  currentPage: number;
}
