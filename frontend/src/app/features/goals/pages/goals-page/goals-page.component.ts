import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GoalService } from '../../services/goal.service';
import { IGoal, GoalCategory, GoalStatus } from '../../models/goal.model';
@Component({
  selector: 'app-goals-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './goals-page.component.html',
  styleUrls: ['./goals-page.component.scss']
})
export class GoalsPageComponent {
  private goalService = inject(GoalService);
  private router = inject(Router);

  Math = Math;

  goals = computed(() => this.goalService.goals());
  activeGoals = computed(() => this.goalService.activeGoals());
  completedGoals = computed(() => this.goalService.completedGoals());

  openGoal(id: number) {
    this.router.navigate(['/goals', id]);
  }

  newGoal() {
    this.router.navigate(['/goals', 'new']);
  }

  getProgress(goal: IGoal): number {
    return this.goalService.getProgress(goal);
  }

  getDaysLeft(goal: IGoal): number | null {
    return this.goalService.getDaysLeft(goal);
  }

  getCategoryColor(category: string): string {
    const colors: Record<string, string> = {
      Work: '#3d5afe',
      Personal: '#7c3aed',
      Health: '#53e076',
      Finance: '#f9c94c',
      Learning: '#06b6d4',
      Other: '#9ca3af',
    };
    return colors[category] ?? '#9ca3af';
  }

  getCompletedCount(goal: IGoal): number {
    return goal.milestones?.filter(m => m.isCompleted).length ?? 0;
  }
}
