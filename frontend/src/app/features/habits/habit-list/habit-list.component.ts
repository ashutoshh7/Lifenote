import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { LucideAngularModule } from 'lucide-angular';
import { Habit } from '../models/habit.model';
import { HabitService } from '../services/habit.service';
import { CheckInDialogComponent } from '../check-in-dialog/check-in-dialog.component';
import { HabitFormComponent } from '../habit-form/habit-form.component';
import { HabitCardComponent } from '../habit-card/habit-card.component';
import {
  Trophy,
  Flame,
  Target,
  CheckCircle2,
  Circle,
  Edit3,
  BarChart2,
  Sparkles,
  Plus,
} from 'lucide-angular';

@Component({
  selector: 'app-habit-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule,
    MatSnackBarModule,
    LucideAngularModule,
    HabitCardComponent,   // ← now uses the redesigned card
  ],
  templateUrl: './habit-list.component.html',
  styleUrl: './habit-list.component.scss',
})
export class HabitListComponent implements OnInit {
  habits: Habit[] = [];
  loading = true;
  completedToday = 0;
  totalHabits = 0;
  completionPercentage = 0;

  // Lucide icon refs
  readonly TrophyIcon        = Trophy;
  readonly FlameIcon         = Flame;
  readonly TargetIcon        = Target;
  readonly CheckCircle2Icon  = CheckCircle2;
  readonly CircleIcon        = Circle;
  readonly EditIcon          = Edit3;
  readonly BarChartIcon      = BarChart2;
  readonly SparklesIcon      = Sparkles;
  readonly PlusIcon          = Plus;

  constructor(
    private readonly habitService: HabitService,
    private readonly dialog: MatDialog,
    private readonly snackBar: MatSnackBar,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadHabits();
  }

  // ── trackBy for ngFor performance ─────────────────────
  trackByHabitId(_index: number, habit: Habit): number {
    return habit.id;
  }

  // ── Data loading ──────────────────────────────────────
  loadHabits(): void {
    this.loading = true;
    this.habitService.getHabits().subscribe({
      next: (habits) => {
        this.habits = habits;
        this.calculateProgress();
        this.loading = false;
        this.cdr.markForCheck(); // OnPush: notify Angular
      },
      error: (error) => {
        console.error('Error loading habits:', error);
        this.loading = false;
        this.cdr.markForCheck();
      },
    });
  }

  calculateProgress(): void {
    this.totalHabits = this.habits.length;
    this.completedToday = this.habits.filter((h) => h.completedToday).length;
    this.completionPercentage =
      this.totalHabits > 0
        ? (this.completedToday / this.totalHabits) * 100
        : 0;
  }

  // ── Check-in ──────────────────────────────────────────
  onCheckIn(habit: Habit): void {
    const dialogConfig: MatDialogConfig<{ habit: Habit }> = {
      width: '100%',
      maxWidth: '440px',
      maxHeight: '90vh',
      data: { habit },
      disableClose: false,
      panelClass: 'check-in-dialog-panel',
    };
    if (typeof window !== 'undefined' && window.innerWidth < 576) {
      dialogConfig.position = { top: '0' };
    }
    const dialogRef = this.dialog.open(CheckInDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.snackBar.open(
          `🎉 Great job! Streak: ${result.log.currentStreak} days`,
          'Dismiss',
          { duration: 5000 },
        );
        this.loadHabits();
      }
    });
  }

  // ── View stats ────────────────────────────────────────
  onViewStats(habit: Habit): void {
    // TODO: navigate to stats page or open stats sheet
    console.log('View stats:', habit.name);
  }

  // ── Create habit ──────────────────────────────────────
  onCreateHabit(): void {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '100%';
    dialogConfig.maxWidth = '440px';
    if (typeof window !== 'undefined' && window.innerWidth < 576) {
      dialogConfig.position = { top: '0px' };
    }
    dialogConfig.maxHeight = '90vh';
    dialogConfig.data = {};
    dialogConfig.disableClose = false;
    dialogConfig.panelClass = 'habit-form-panel';
    const dialogRef = this.dialog.open(HabitFormComponent, dialogConfig);
    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.snackBar.open(
          `✅ "${result.habit.name}" created`,
          'Dismiss',
          { duration: 4000 },
        );
        this.loadHabits();
      }
    });
  }

  // ── Edit habit ────────────────────────────────────────
  onEditHabit(habit: Habit): void {
    const dialogRef = this.dialog.open(HabitFormComponent, {
      width: '100%',
      maxWidth: '440px',
      maxHeight: '90vh',
      data: { habit },
      disableClose: false,
      panelClass: 'habit-form-panel',
    });
    dialogRef.afterClosed().subscribe((result) => {
      if (result?.success) {
        this.snackBar.open(
          `✅ "${result.habit.name}" updated`,
          'Dismiss',
          { duration: 4000 },
        );
        this.loadHabits();
      }
    });
  }

  // Legacy helper (kept for any templates that still use it)
  getCountArray(n: number): number[] {
    return Array.from({ length: Math.max(0, n) }, (_, i) => i);
  }
}
