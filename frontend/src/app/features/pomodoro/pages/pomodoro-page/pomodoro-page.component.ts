import {
  Component,
  inject,
  ViewChild,
  ViewChildren,
  QueryList,
  ElementRef,
  AfterViewChecked,
  OnInit,
  OnDestroy,
  signal,
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
import { MobileFabComponent } from '../../../../shared';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-pomodoro-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, MobileFabComponent],
  templateUrl: './pomodoro-page.component.html',
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

  @ViewChild('carousel') carouselRef!: ElementRef<HTMLDivElement>;
  @ViewChildren('titleInput') titleInputs!: QueryList<ElementRef<HTMLInputElement>>;

  canScrollLeft = false;
  canScrollRight = false;
  private lastScrollWidth = 0;

  ngOnInit(): void {
    this.loadFocusStats();
    this.completionSub = this.pomodoroService.getCompletion$().subscribe(() => {
      this.loadFocusStats();
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
    if (this.carouselRef) {
      const el = this.carouselRef.nativeElement;
      if (el.scrollWidth !== this.lastScrollWidth) {
        this.lastScrollWidth = el.scrollWidth;
        setTimeout(() => this.updateScrollState(), 0);
      }
    }

    if (!this.focusPending || !this.titleInputs?.length) return;
    const el = this.titleInputs.first?.nativeElement;
    if (el) {
      el.focus();
      el.select();
      this.focusPending = false;
    }
  }

  updateScrollState(): void {
    if (!this.carouselRef) return;
    const el = this.carouselRef.nativeElement;
    this.canScrollLeft = el.scrollLeft > 2;
    this.canScrollRight = Math.ceil(el.scrollLeft + el.clientWidth) < el.scrollWidth - 2;
  }

  addTimer(): void {
    this.pomodoroService.addTimer();
    setTimeout(() => {
      if (this.carouselRef) {
        const el = this.carouselRef.nativeElement;
        el.scrollTo({ left: el.scrollWidth, behavior: 'smooth' });
      }
    }, 50);
  }

  scrollLeft(): void {
    if (this.carouselRef) {
      const el = this.carouselRef.nativeElement;
      const child = el.firstElementChild as HTMLElement;
      if (child) {
        const itemWidth = child.offsetWidth + 20; // 20px is the gap
        el.scrollBy({ left: -itemWidth, behavior: 'smooth' });
      }
    }
  }

  scrollRight(): void {
    if (this.carouselRef) {
      const el = this.carouselRef.nativeElement;
      const child = el.firstElementChild as HTMLElement;
      if (child) {
        const itemWidth = child.offsetWidth + 20;
        el.scrollBy({ left: itemWidth, behavior: 'smooth' });
      }
    }
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
    if (isNaN(val)) {
      val = 0;
    }
    if (val < min) val = min;
    if (val > max) val = max;
    
    if (field === 'hours') this.editHours = val;
    else if (field === 'minutes') this.editMinutes = val;
    else if (field === 'seconds') this.editSeconds = val;
    
    // Enforce the value in the input field UI
    input.value = val.toString();
  }
}
