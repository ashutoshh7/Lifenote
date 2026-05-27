import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IGoal } from '../../../features/goals/models/goal.model';
import { GoalService } from '../../../features/goals/services/goal.service';

@Component({
  selector: 'app-goal-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './goal-card.component.html',
  styleUrls: ['./goal-card.component.scss']
})
export class GoalCardComponent {
  private goalService = inject(GoalService);

  @Input({ required: true }) goal!: IGoal;
  @Input() variant: 'mini' | 'detailed' = 'detailed';
  
  @Output() cardClick = new EventEmitter<string>();

  Math = Math;

  onClick(event: Event) {
    // Only emit if it's the card itself, let edit button handle its own event
    this.cardClick.emit(this.goal.id);
  }

  onEditClick(event: Event) {
    event.stopPropagation();
    this.cardClick.emit(this.goal.id); // For now edit also just goes to the goal editor
  }

  get progress(): number {
    return this.goalService.getProgress(this.goal);
  }

  get daysLeft(): number | null {
    return this.goalService.getDaysLeft(this.goal);
  }

  get completedCount(): number {
    return this.goalService.getCompletedMilestonesCount(this.goal);
  }

  get categoryColor(): string {
    return this.goalService.getCategoryColor(this.goal.category);
  }
}
