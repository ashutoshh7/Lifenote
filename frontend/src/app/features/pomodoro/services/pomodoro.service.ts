import { Injectable } from '@angular/core';
import { BehaviorSubject, interval, Subscription, Observable, Subject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

export type PomodoroType = 'pomodoro' | 'short-break' | 'long-break';

export interface PomodoroTimer {
  id: string;
  label: string;
  type: PomodoroType;
  hours: number;
  minutes: number;
  seconds: number;
  running: boolean;
  discharged?: boolean;
}

const DEFAULT_DURATIONS: Record<PomodoroType, number> = {
  'pomodoro': 25,
  'short-break': 5,
  'long-break': 15
};

function generateId(): string {
  return 'pomo-' + Date.now() + '-' + Math.random().toString(36).slice(2, 9);
}

@Injectable({
  providedIn: 'root'
})
export class PomodoroService {
  private startAudio = new Audio('../../../../assets/sounds/omnitrix-transform.mp3');
  private endAudio = new Audio('../../../../assets/sounds/omnitrix-power-down.mp3');

  private timers$ = new BehaviorSubject<PomodoroTimer[]>([]);
  private tickSub: Subscription | null = null;

  private completion$ = new Subject<void>();

  getCompletion$() {
    return this.completion$.asObservable();
  }

  /** All timers; subscribe in components. */
  getTimers$() {
    return this.timers$.asObservable();
  }

  get timers(): PomodoroTimer[] {
    return this.timers$.value;
  }

  constructor(private http: HttpClient) {
    this.ensureAtLeastOne();
    this.loadActiveTimer();
  }

  private loadActiveTimer(): void {
    this.http.get<any>(`${environment.apiHost}/Timer/active`).subscribe({
      next: (data) => {
        if (data && data.isRunning) {
          // Sync with backend state
          if (this.timers$.value.length > 0) {
            const id = this.timers$.value[0].id;
            this.setTime(id, 0, Math.floor(data.remainingSeconds / 60), data.remainingSeconds % 60);
            this.updateTimer(id, t => ({ ...t, running: true, discharged: false }));
            this.startTickIfNeeded();
          }
        }
      },
      error: (err) => console.error('Failed to load active timer', err)
    });
  }

  private ensureAtLeastOne(): void {
    if (this.timers$.value.length === 0) {
      this.addTimer();
    }
  }

  addTimer(): PomodoroTimer {
    const id = generateId();
    const type: PomodoroType = 'pomodoro';
    const timer: PomodoroTimer = {
      id,
      label: `Deep Work ${this.timers$.value.length + 1}`,
      type,
      hours: 0,
      minutes: 0, // default to 15 seconds as requested
      seconds: 15,
      running: false,
      discharged: false
    };
    this.timers$.next([...this.timers$.value, timer]);
    this.startTickIfNeeded();
    return timer;
  }

  removeTimer(id: string): void {
    const list = this.timers$.value.filter(t => t.id !== id);
    if (list.length === 0) {
      this.addTimer();
      return;
    }
    this.timers$.next(list);
    this.stopTickIfIdle();
  }

  setType(id: string, type: PomodoroType): void {
    this.updateTimer(id, t => ({
      ...t,
      type,
      hours: 0,
      minutes: DEFAULT_DURATIONS[type],
      seconds: 0,
      running: false,
      discharged: false
    }));
  }

  setLabel(id: string, label: string): void {
    this.updateTimer(id, t => ({ ...t, label: label.trim() || t.label }));
  }

  private playAudio(audio: HTMLAudioElement): void {
    // This ensures the browser knows we are attempting to play from a known element
    audio.currentTime = 0;
    audio.play().then(() => {
      console.log('Audio playing successfully:', audio.src);
    }).catch(e => {
      console.error('Audio play failed for', audio.src, e);
      // Try a secondary fallback if it was garbage collected or blocked
      const fallback = new Audio(audio.src);
      fallback.play().catch(err => console.error('Fallback audio also failed:', err));
    });
  }

  start(id: string): void {
    const tInfo = this.timers$.value.find(t => t.id === id);
    if (tInfo && !tInfo.running) {
      // this.playAudio(this.startAudio);
      this.http.post(`${environment.apiHost}/Timer/start`, {
        durationSeconds: (tInfo.hours * 3600) + (tInfo.minutes * 60) + tInfo.seconds,
        focusModeEnabled: true,
        sessionType: tInfo.type === 'pomodoro' ? 0 : (tInfo.type === 'short-break' ? 1 : 2)
      }).subscribe({
        error: (err) => console.error('Failed to start timer on server', err)
      });
    }
    this.updateTimer(id, t => ({ ...t, running: true, discharged: false }));
    this.startTickIfNeeded();
  }

  stop(id: string): void {
    const tInfo = this.timers$.value.find(t => t.id === id);
    if (tInfo) {
      const remainingSeconds = (tInfo.hours * 3600) + (tInfo.minutes * 60) + tInfo.seconds;
      this.http.post(`${environment.apiHost}/Timer/pause`, { remainingSeconds }).subscribe({
        error: (err) => console.error('Failed to pause timer on server', err)
      });
    }
    this.updateTimer(id, t => ({ ...t, running: false }));
    this.stopTickIfIdle();
  }

  reset(id: string): void {
    const type = this.timers$.value.find(t => t.id === id)?.type ?? 'pomodoro';
    this.http.post(`${environment.apiHost}/Timer/reset`, {}).subscribe({
      error: (err) => console.error('Failed to reset timer on server', err)
    });
    this.updateTimer(id, t => ({
      ...t,
      running: false,
      discharged: false,
      hours: 0,
      minutes: 0, // Reset to 15 seconds
      seconds: 15
    }));
  }

  setTime(id: string, hours: number, minutes: number, seconds: number): void {
    this.updateTimer(id, t => ({
      ...t,
      hours: Math.max(0, hours),
      minutes: Math.max(0, Math.min(59, minutes)),
      seconds: Math.max(0, Math.min(59, seconds)),
      running: false,
      discharged: false
    }));
  }

  formatTime(timer: PomodoroTimer): string {
    const h = timer.hours > 0 ? timer.hours.toString().padStart(2, '0') + ':' : '';
    const m = timer.minutes.toString().padStart(2, '0');
    const s = timer.seconds.toString().padStart(2, '0');
    return `${h}${m}:${s}`;
  }

  private updateTimer(id: string, fn: (t: PomodoroTimer) => PomodoroTimer): void {
    this.timers$.next(
      this.timers$.value.map(t => t.id === id ? fn(t) : t)
    );
  }

  private startTickIfNeeded(): void {
    if (this.tickSub || !this.timers$.value.some(t => t.running)) return;
    this.tickSub = interval(1000).subscribe(() => this.tick());
  }

  private stopTickIfIdle(): void {
    if (!this.timers$.value.some(t => t.running) && this.tickSub) {
      this.tickSub.unsubscribe();
      this.tickSub = null;
    }
  }

  private tick(): void {
    const next = this.timers$.value.map(t => {
      if (!t.running) return t;

      if (t.hours === 0 && t.minutes === 0 && t.seconds === 4) {
        this.playAudio(this.endAudio);
      }

      let { hours, minutes, seconds } = t;
      if (seconds > 0) seconds--;
      else if (minutes > 0) {
        minutes--;
        seconds = 59;
      } else if (hours > 0) {
        hours--;
        minutes = 59;
        seconds = 59;
      } else {
        this.http.post(`${environment.apiHost}/Timer/complete`, {}).subscribe({
          next: () => this.completion$.next(),
          error: (err) => console.error('Failed to complete timer on server', err)
        });
        return { ...t, running: false, discharged: true, hours: 0, minutes: 0, seconds: 15 };
      }
      return { ...t, hours, minutes, seconds };
    });
    this.timers$.next(next);
    this.stopTickIfIdle();
  }

  getFocusStats(): Observable<any> {
    return this.http.get<any>(`${environment.apiHost}/FocusSession/stats`);
  }
}
