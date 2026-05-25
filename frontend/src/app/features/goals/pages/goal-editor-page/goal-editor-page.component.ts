import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GoalService } from '../../services/goal.service';
import { ToastService } from '../../../../core/services/toast.service';
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
  private toastService = inject(ToastService);

  isNew = signal(false);
  goal = signal<IGoal | null>(null);
  showAddMilestone = signal(false);

  // Loading / spinner states
  isSaving = signal(false);
  isSavingMilestone = signal(false);

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
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id === 'new') {
        this.isNew.set(true);
        this.goal.set(null);
        this.title.set('');
        this.description.set('');
        this.category.set('Work');
        this.status.set('Active');
        this.targetDate.set('');
      } else {
        this.isNew.set(false);
        const goalId = parseInt(id ?? '', 10);
        this.goalService.getGoalById(goalId).subscribe({
          next: (found) => {
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
          },
          error: () => this.router.navigate(['/goals'])
        });
      }
    });

    // Handle opening add milestone form if goal was just created
    this.route.queryParams.subscribe(qParams => {
      if (qParams['created'] === 'true') {
        this.showAddMilestone.set(true);
      }
    });
  }

  save() {
    if (!this.title().trim()) return;
    this.isSaving.set(true);

    if (this.isNew()) {
      const dto: ICreateGoalDto = {
        title: this.title(),
        description: this.description(),
        category: this.category(),
        status: this.status(),
        targetDate: this.targetDate() || undefined,
      };
      this.goalService.createGoal(dto).subscribe({
        next: (created) => {
          this.toastService.show('Goal added successfully!');
          this.isSaving.set(false);
          this.router.navigate(['/goals', created.id], { queryParams: { created: 'true' } });
        },
        error: () => {
          this.toastService.show('Failed to save goal', 'error');
          this.isSaving.set(false);
        }
      });
    } else if (this.goal()) {
      this.goalService.updateGoal(this.goal()!.id, {
        title: this.title(),
        description: this.description(),
        category: this.category(),
        status: this.status(),
        targetDate: this.targetDate() || undefined,
      }).subscribe({
        next: (updated) => {
          this.toastService.show('Goal updated successfully!');
          this.goal.set(updated);
          this.isSaving.set(false);
        },
        error: () => {
          this.toastService.show('Failed to update goal', 'error');
          this.isSaving.set(false);
        }
      });
    }
  }

  discard() {
    this.router.navigate(['/goals']);
  }

  toggleMilestone(milestoneId: number) {
    const g = this.goal();
    if (g) {
      this.goalService.toggleMilestone(g.id, milestoneId).subscribe(() => {
        this.goalService.getGoalById(g.id).subscribe(updated => {
          this.goal.set(updated);
        });
      });
    }
  }

  deleteMilestone(milestoneId: number) {
    const g = this.goal();
    if (g) {
      this.goalService.deleteMilestone(g.id, milestoneId).subscribe(() => {
        this.goalService.getGoalById(g.id).subscribe(updated => {
          this.goal.set(updated);
        });
      });
    }
  }

  openAddMilestone() {
    this.showAddMilestone.set(true);
    setTimeout(() => {
      const el = document.getElementById('add-milestone-form');
      if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }, 100);
  }

  submitMilestone() {
    const g = this.goal();
    if (!g || !this.newMilestoneTitle().trim()) return;
    this.isSavingMilestone.set(true);

    this.goalService.addMilestone(g.id, {
      goalId: g.id,
      title: this.newMilestoneTitle(),
      description: this.newMilestoneDesc(),
      targetDate: this.newMilestoneDate() || undefined,
    }).subscribe({
      next: () => {
        this.toastService.show('Milestone added successfully!');
        this.goalService.getGoalById(g.id).subscribe(updated => {
          this.goal.set(updated);
        });
        this.newMilestoneTitle.set('');
        this.newMilestoneDate.set('');
        this.newMilestoneDesc.set('');
        this.showAddMilestone.set(false);
        this.isSavingMilestone.set(false);
      },
      error: () => {
        this.toastService.show('Failed to add milestone', 'error');
        this.isSavingMilestone.set(false);
      }
    });
  }

  getCategoryColor(category: string): string {
    const colors: Record<string, string> = {
      Work: '#3d5afe', Personal: '#7c3aed', Health: '#53e076',
      Finance: '#f9c94c', Learning: '#06b6d4', Other: '#9ca3af',
    };
    return colors[category] ?? '#9ca3af';
  }
}
