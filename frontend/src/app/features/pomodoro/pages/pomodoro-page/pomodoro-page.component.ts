import {
  Component,
  inject,
  ViewChildren,
  QueryList,
  ElementRef,
  OnInit,
  OnDestroy,
  AfterViewChecked,
  signal,
  computed,
  ChangeDetectionStrategy
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import { Subscription } from 'rxjs';
import {
  PomodoroService,
  PomodoroTimer,
  PomodoroType,
} from '../../services/pomodoro.service';
import { ToastService } from '../../../../core/services/toast.service';
import { DurationPickerComponent } from '../../components/duration-picker/duration-picker.component';

@Component({
  selector: 'app-pomodoro-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, DurationPickerComponent],
  templateUrl: './pomodoro-page.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./pomodoro-page.component.scss'],
})
export class PomodoroPageComponent implements OnInit, OnDestroy, AfterViewChecked {
  private pomodoroService = inject(PomodoroService);
  private toastService = inject(ToastService);

  timers$ = this.pomodoroService.getTimers$();

  todayFocusHours = signal<number>(0);
  currentStreak = signal<number>(0);
  private completionSub: Subscription | null = null;

  editingTimerId = signal<string | null>(null);
  editingLabel = '';
  private focusPending = false;
  selectedTimerId = signal<string | null>(null);

  // Time Duration Editing State
  editingTimer = signal<PomodoroTimer | null>(null);

  @ViewChildren('titleInput') titleInputs!: QueryList<ElementRef<HTMLInputElement>>;

  ngOnInit(): void {
    this.loadFocusStats();
    this.completionSub = this.pomodoroService.getCompletion$().subscribe(() => {
      this.loadFocusStats();
    });

    // Auto-select running timer or first timer if none selected
    this.timers$.subscribe(timers => {
      if (timers.length > 0) {
        const runningTimer = timers.find(t => t.running);

        if (!this.selectedTimerId()) {
          this.selectedTimerId.set(runningTimer ? runningTimer.id : timers[0].id);
        } else {
          // If selected timer no longer exists, re-select
          if (!timers.find(t => t.id === this.selectedTimerId())) {
            this.selectedTimerId.set(runningTimer ? runningTimer.id : timers[0].id);
          } else if (runningTimer && this.selectedTimerId() !== runningTimer.id) {
            // Since playlist is hidden, force switch to running timer if it exists
            this.selectedTimerId.set(runningTimer.id);
          }
        }
      }
    });
  }

  ngOnDestroy(): void {
    if (this.completionSub) {
      this.completionSub.unsubscribe();
    }
  }

  loadFocusStats(): void {
    this.pomodoroService.getFocusStats().subscribe({
      next: (res: any) => {
        if (res && res.data) {
          this.todayFocusHours.set(res.data.todayFocusHours || 0);
          this.currentStreak.set(res.data.currentStreak || 0);
        }
      },
      error: (err: any) => this.toastService.show('Failed to load focus stats', 'error')
    });
  }

  ngAfterViewChecked(): void {
    if (!this.focusPending || !this.titleInputs?.length) return;
    const el = this.titleInputs.first?.nativeElement;
    if (el) {
      el.focus();
      el.select();
      this.focusPending = false;
    }
  }

  addTimer(): void {
    this.pomodoroService.addTimer();
    this.toastService.show('New timer added successfully.', 'success');
  }

  selectTimer(id: string): void {
    this.selectedTimerId.set(id);
  }

  removeTimer(id: string): void {
    this.pomodoroService.removeTimer(id);
    if (this.editingTimerId() === id) this.cancelEditLabel();
  }

  setType(id: string, type: PomodoroType): void {
    this.pomodoroService.setType(id, type);
  }

  startEditLabel(timer: PomodoroTimer): void {
    this.editingTimerId.set(timer.id);
    this.editingLabel = timer.label;
    this.focusPending = true;
  }

  saveLabel(id: string): void {
    if (this.editingTimerId() !== id) return;
    const label = this.editingLabel.trim();
    if (label) this.pomodoroService.setLabel(id, label);
    this.editingTimerId.set(null);
    this.editingLabel = '';
  }

  cancelEditLabel(): void {
    this.editingTimerId.set(null);
    this.editingLabel = '';
  }

  start(id: string): void {
    this.pomodoroService.start(id);
  }

  stop(id: string): void {
    this.pomodoroService.stop(id);
  }

  reset(id: string): void {
    this.pomodoroService.reset(id);
  }

  formatTime(timer: PomodoroTimer): string {
    return this.pomodoroService.formatTime(timer);
  }

  startEditDuration(timer: PomodoroTimer): void {
    this.editingTimer.set(timer);
  }

  saveDuration(duration: { hours: number; minutes: number; seconds: number }): void {
    const timer = this.editingTimer();
    if (!timer) return;

    let h = Math.max(0, duration.hours);
    let m = Math.max(0, duration.minutes);
    let s = Math.max(0, duration.seconds);

    // Normalize seconds and minutes if they exceed 59
    if (s >= 60) {
      m += Math.floor(s / 60);
      s = s % 60;
    }
    if (m >= 60) {
      h += Math.floor(m / 60);
      m = m % 60;
    }

    // Default fallback to 15 seconds if total time is 0
    if (h === 0 && m === 0 && s === 0) {
      s = 15;
    }

    this.pomodoroService.setTime(timer.id, h, m, s);
    this.editingTimer.set(null);
  }

  cancelEditDuration(): void {
    this.editingTimer.set(null);
  }
}

