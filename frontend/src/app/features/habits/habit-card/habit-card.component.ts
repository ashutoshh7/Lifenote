import {
  Component,
  ChangeDetectionStrategy,
  input,
  output,
  computed,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import {
  Flame,
  CheckCircle2,
  Circle,
  Edit3,
  BarChart2,
} from 'lucide-angular';
import { Habit } from '../models/habit.model';

@Component({
  selector: 'app-habit-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './habit-card.component.html',
  styleUrl: './habit-card.component.scss',
})
export class HabitCardComponent {
  // ── Inputs (Angular signals API) ──────────────────────
  habit = input.required<Habit>();

  // ── Outputs ───────────────────────────────────────────
  checkIn  = output<Habit>();
  edit     = output<Habit>();
  viewStats = output<Habit>();

  // ── Lucide icons ──────────────────────────────────────
  readonly FlameIcon        = Flame;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly CircleIcon       = Circle;
  readonly EditIcon         = Edit3;
  readonly BarChartIcon     = BarChart2;

  // ── Computed helpers ──────────────────────────────────
  readonly isCompleted = computed(() => this.habit().completedToday);
  readonly isMaxedOut  = computed(
    () => this.habit().completedCountToday >= this.habit().targetCount
  );
  readonly hasStreak   = computed(() => this.habit().currentStreak > 0);
  readonly showDots    = computed(() => this.habit().targetCount > 1);

  readonly dotArray = computed(() =>
    Array.from({ length: this.habit().targetCount }, (_, i) => i)
  );

  readonly progressPct = computed(() => {
    const h = this.habit();
    return h.targetCount > 0
      ? Math.min(100, (h.completedCountToday / h.targetCount) * 100)
      : 0;
  });

  // ── Event handlers ────────────────────────────────────
  onCheckIn(): void {
    if (!this.isMaxedOut()) {
      this.checkIn.emit(this.habit());
    }
  }

  onEdit(): void {
    this.edit.emit(this.habit());
  }

  onViewStats(): void {
    this.viewStats.emit(this.habit());
  }
}
