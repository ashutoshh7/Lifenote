import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GoalService } from '../../services/goal.service';
import { IGoal, IMilestone, GoalCategory, GoalStatus, ICreateGoalDto } from '../../models/goal.model';

@Component({
  selector: 'app-goal-editor-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './goal-editor-page.component.html',
  styleUrls: ['./goal-editor-page.component.scss']
})
export class GoalEditorPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private goalService = inject(GoalService);

  isNew = signal(false);
  goal = signal<IGoal | null>(null);
  showAddMilestone = signal(false);

  // Form fields
  title = signal('');
  description = signal('');
  category = signal<GoalCategory>('Work');
  status = signal<GoalStatus>('Active');
  targetDate = signal('');

  // New milestone form
  newMilestoneTitle = signal('');
  newMilestoneDate = signal('');
  newMilestoneDesc = signal('');

  categories: GoalCategory[] = ['Work', 'Personal', 'Health', 'Finance', 'Learning', 'Other'];
  statuses: GoalStatus[] = ['Active', 'Paused', 'Completed', 'Archived'];

  progress = computed(() => {
    const g = this.goal();
    return g ? this.goalService.getProgress(g) : 0;
  });

  daysLeft = computed(() => {
    const g = this.goal();
    return g ? this.goalService.getDaysLeft(g) : null;
  });

  milestones = computed(() => this.goal()?.milestones ?? []);
  completedMilestonesCount = computed(() => this.milestones().filter(m => m.isCompleted).length);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id === 'new') {
      this.isNew.set(true);
    } else {
      const goalId = parseInt(id ?? '', 10);
      const found = this.goalService.getGoalById(goalId);
      if (found) {
        this.goal.set(found);
        this.title.set(found.title);
        this.description.set(found.description ?? '');
        this.category.set(found.category);
        this.status.set(found.status);
        this.targetDate.set(found.targetDate ? found.targetDate.substring(0, 10) : '');
      } else {
        this.router.navigate(['/goals']);
      }
    }
  }

  save() {
    if (!this.title().trim()) return;

    if (this.isNew()) {
      const dto: ICreateGoalDto = {
        title: this.title(),
        description: this.description(),
        category: this.category(),
        status: this.status(),
        targetDate: this.targetDate() || undefined,
      };
      const created = this.goalService.createGoal(dto);
      this.router.navigate(['/goals', created.id]);
    } else if (this.goal()) {
      this.goalService.updateGoal(this.goal()!.id, {
        title: this.title(),
        description: this.description(),
        category: this.category(),
        status: this.status(),
        targetDate: this.targetDate() || undefined,
      });
      // Refresh
      this.goal.set(this.goalService.getGoalById(this.goal()!.id) ?? null);
    }
  }

  discard() {
    this.router.navigate(['/goals']);
  }

  toggleMilestone(milestoneId: number) {
    const g = this.goal();
    if (g) {
      this.goalService.toggleMilestone(g.id, milestoneId);
      this.goal.set(this.goalService.getGoalById(g.id) ?? null);
    }
  }

  deleteMilestone(milestoneId: number) {
    const g = this.goal();
    if (g) {
      this.goalService.deleteMilestone(g.id, milestoneId);
      this.goal.set(this.goalService.getGoalById(g.id) ?? null);
    }
  }

  submitMilestone() {
    const g = this.goal();
    if (!g || !this.newMilestoneTitle().trim()) return;

    this.goalService.addMilestone({
      goalId: g.id,
      title: this.newMilestoneTitle(),
      description: this.newMilestoneDesc(),
      targetDate: this.newMilestoneDate() || undefined,
    });

    this.goal.set(this.goalService.getGoalById(g.id) ?? null);
    this.newMilestoneTitle.set('');
    this.newMilestoneDate.set('');
    this.newMilestoneDesc.set('');
    this.showAddMilestone.set(false);
  }

  getCategoryColor(category: string): string {
    const colors: Record<string, string> = {
      Work: '#3d5afe', Personal: '#7c3aed', Health: '#53e076',
      Finance: '#f9c94c', Learning: '#06b6d4', Other: '#9ca3af',
    };
    return colors[category] ?? '#9ca3af';
  }
}
