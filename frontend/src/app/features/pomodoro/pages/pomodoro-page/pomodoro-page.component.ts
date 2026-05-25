import {
  Component,
  inject,
  ViewChildren,
  QueryList,
  ElementRef,
  AfterViewChecked,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule } from 'lucide-angular';
import {
  PomodoroService,
  PomodoroTimer,
  PomodoroType,
} from '../../services/pomodoro.service';
import { PageHeaderComponent } from '../../../../shared';

@Component({
  selector: 'app-pomodoro-page',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule, PageHeaderComponent],
  templateUrl: './pomodoro-page.component.html',
  styleUrls: ['./pomodoro-page.component.scss'],
})
export class PomodoroPageComponent implements AfterViewChecked {
  private pomodoroService = inject(PomodoroService);

  timers$ = this.pomodoroService.getTimers$();

  editingTimerId: string | null = null;
  editingLabel = '';
  private focusPending = false;

  @ViewChildren('titleInput') titleInputs!: QueryList<ElementRef<HTMLInputElement>>;

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
}
