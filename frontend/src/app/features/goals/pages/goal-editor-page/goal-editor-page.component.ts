import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GoalService } from '../../services/goal.service';
import { ToastService } from '../../../../core/services/toast.service';
import { IGoal, IMilestone, GoalCategory, GoalStatus, ICreateGoalDto } from '../../models/goal.model';
import { GOAL_CATEGORIES, GOAL_STATUSES, GOAL_CATEGORY_COLORS } from '../../../../core/constants/goal.constants';

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
  showMobileMilestones = signal(false);

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

  categories = GOAL_CATEGORIES;
  statuses = GOAL_STATUSES;

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
        const goalId = id ?? '';
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
      const gId = this.goal()!.id;
      this.goalService.updateGoal(gId, {
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
          this.router.navigate(['/goals'], { queryParams: { highlight: gId } });
        },
        error: () => {
          this.toastService.show('Failed to update goal', 'error');
          this.isSaving.set(false);
        }
      });
    }
  }

  discard() {
    const g = this.goal();
    if (!this.isNew() && g) {
      this.router.navigate(['/goals'], { queryParams: { highlight: g.id } });
    } else {
      this.router.navigate(['/goals']);
    }
  }

  deleteGoal() {
    const currentGoal = this.goal();
    if (!currentGoal) return;

    if (confirm('Are you sure you want to delete this goal? All associated milestones will be permanently lost.')) {
      this.isSaving.set(true);
      this.goalService.deleteGoal(currentGoal.id).subscribe({
        next: () => {
          this.toastService.show('Goal deleted successfully!');
          this.isSaving.set(false);
          this.router.navigate(['/goals']);
        },
        error: () => {
          this.toastService.show('Failed to delete goal', 'error');
          this.isSaving.set(false);
        }
      });
    }
  }

  toggleMilestone(milestoneId: string) {
    const g = this.goal();
    if (g) {
      this.goalService.toggleMilestone(g.id, milestoneId).subscribe(() => {
        this.goalService.getGoalById(g.id).subscribe(updated => {
          this.goal.set(updated);
        });
      });
    }
  }

  deleteMilestone(milestoneId: string) {
    const g = this.goal();
    if (g) {
      this.goalService.deleteMilestone(g.id, milestoneId).subscribe(() => {
        this.goalService.getGoalById(g.id).subscribe(updated => {
          this.goal.set(updated);
        });
      });
    }
  }

  editingMilestoneId = signal<string | null>(null);

  startEditMilestone(milestone: IMilestone) {
    this.editingMilestoneId.set(milestone.id);
    this.newMilestoneTitle.set(milestone.title);
    this.newMilestoneDate.set(milestone.targetDate ? milestone.targetDate.substring(0, 10) : '');
    this.newMilestoneDesc.set(milestone.description ?? '');
    this.showAddMilestone.set(true);
    setTimeout(() => {
      const el = document.getElementById('add-milestone-form');
      if (el) {
        el.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }, 100);
  }

  openAddMilestone() {
    this.editingMilestoneId.set(null);
    this.newMilestoneTitle.set('');
    this.newMilestoneDate.set('');
    this.newMilestoneDesc.set('');
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

    const title = this.newMilestoneTitle();
    const description = this.newMilestoneDesc();
    const targetDate = this.newMilestoneDate() || undefined;

    const editId = this.editingMilestoneId();
    if (editId) {
      this.goalService.updateMilestone(g.id, editId, {
        title,
        description,
        targetDate,
        isCompleted: g.milestones.find(m => m.id === editId)?.isCompleted ?? false
      }).subscribe({
        next: () => {
          this.toastService.show('Milestone updated successfully!');
          this.goalService.getGoalById(g.id).subscribe(updated => {
            this.goal.set(updated);
          });
          this.editingMilestoneId.set(null);
          this.newMilestoneTitle.set('');
          this.newMilestoneDate.set('');
          this.newMilestoneDesc.set('');
          this.showAddMilestone.set(false);
          this.isSavingMilestone.set(false);
        },
        error: () => {
          this.toastService.show('Failed to update milestone', 'error');
          this.isSavingMilestone.set(false);
        }
      });
    } else {
      this.goalService.addMilestone(g.id, {
        goalId: g.id,
        title,
        description,
        targetDate,
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
  }

  getCategoryColor(category: string): string {
    return GOAL_CATEGORY_COLORS[category] ?? '#9ca3af';
  }
}
