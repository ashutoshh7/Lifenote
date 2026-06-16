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

@Component({
  selector: 'app-pomodoro-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
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

  editingTimerId: string | null = null;
  editingLabel = '';
  private focusPending = false;
  selectedTimerId = signal<string | null>(null);

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
    if (this.editingTimerId === id) this.cancelEditLabel();
  }

  setType(id: string, type: PomodoroType): void {
    this.pomodoroService.setType(id, type);
  }

  startEditLabel(timer: PomodoroTimer): void {
    this.editingTimerId = timer.id;
    this.editingLabel = timer.label;
    this.focusPending = true;
  }

  saveLabel(id: string): void {
    if (this.editingTimerId !== id) return;
    const label = this.editingLabel.trim();
    if (label) this.pomodoroService.setLabel(id, label);
    this.editingTimerId = null;
    this.editingLabel = '';
  }

  cancelEditLabel(): void {
    this.editingTimerId = null;
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

  editingDurationTimerId: string | null = null;
  editHours = 0;
  editMinutes = 25;
  editSeconds = 0;

  hoursList = Array.from({ length: 24 }, (_, i) => i);
  minutesList = Array.from({ length: 60 }, (_, i) => i);
  secondsList = Array.from({ length: 60 }, (_, i) => i);

  startEditDuration(timer: PomodoroTimer): void {
    this.editingDurationTimerId = timer.id;
    this.editHours = timer.hours || 0;
    this.editMinutes = timer.minutes;
    this.editSeconds = timer.seconds;
  }

  saveDuration(id: string): void {
    if (this.editingDurationTimerId !== id) return;

    let h = Math.max(0, parseInt(this.editHours as any, 10) || 0);
    let m = Math.max(0, parseInt(this.editMinutes as any, 10) || 0);
    let s = Math.max(0, parseInt(this.editSeconds as any, 10) || 0);

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

    this.pomodoroService.setTime(id, h, m, s);
    this.editingDurationTimerId = null;
  }

  cancelEditDuration(): void {
    this.editingDurationTimerId = null;
  }

  limitValue(event: Event, min: number, max: number, field: 'hours' | 'minutes' | 'seconds'): void {
    const input = event.target as HTMLInputElement;
    let val = parseInt(input.value, 10);
    if (isNaN(val)) val = 0;
    if (val < min) val = min;
    if (val > max) val = max;

    if (field === 'hours') this.editHours = val;
    else if (field === 'minutes') this.editMinutes = val;
    else if (field === 'seconds') this.editSeconds = val;

    input.value = val.toString();
  }

  onDrumScroll(event: Event, type: 'hours' | 'minutes' | 'seconds') {
    const el = event.target as HTMLElement;
    const itemHeight = 40;
    const index = Math.round(el.scrollTop / itemHeight);

    if (type === 'hours') {
      this.editHours = Math.min(Math.max(0, index), 23);
    } else if (type === 'minutes') {
      this.editMinutes = Math.min(Math.max(0, index), 59);
    } else if (type === 'seconds') {
      this.editSeconds = Math.min(Math.max(0, index), 59);
    }
  }

  onDrumWheel(event: WheelEvent, type: 'hours' | 'minutes' | 'seconds') {
    event.preventDefault();
    const el = event.currentTarget as HTMLElement;
    const itemHeight = 40;

    const direction = Math.sign(event.deltaY);
    const currentSnap = Math.round(el.scrollTop / itemHeight);
    const nextSnap = currentSnap + direction;

    el.scrollTo({
      top: nextSnap * itemHeight,
      behavior: 'smooth'
    });
  }
}
