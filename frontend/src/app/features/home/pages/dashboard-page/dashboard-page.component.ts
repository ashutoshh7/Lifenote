import { Component, inject, computed, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { NotesService } from '../../../notes/pages/notes-page/note-page-services/notes.service';
import { AuthService } from '../../../../core/services/auth.service';
import { INote } from '../../../../core/models/note.model';
import { PageHeaderComponent } from '../../../../shared';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterModule, PageHeaderComponent],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  private notesService = inject(NotesService);
  authService = inject(AuthService);
  private router = inject(Router);

  timerSeconds = signal(24 * 60 + 59);
  timerRunning = signal(false);
  private timerInterval: ReturnType<typeof setInterval> | null = null;

  recentNotes = computed(() => this.notesService.notes().slice(0, 4));
  totalNotes = computed(() => this.notesService.notes().length);

  get greeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning';
    if (hour < 18) return 'Good afternoon';
    return 'Good evening';
  }

  get userName(): string {
    const details = this.authService.currentUserDetails();
    if (details?.firstName) return details.firstName;
    if (details?.username) return details.username;
    return 'there';
  }

  get formattedTime(): string {
    const mins = Math.floor(this.timerSeconds() / 60);
    const secs = this.timerSeconds() % 60;
    return `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;
  }

  ngOnInit() {
    this.notesService.getAllNotes().subscribe();
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
  }

  startSession() {
    this.router.navigate(['/pomodoro']);
  }

  getNoteIcon(note: INote): string {
    const content = note.content?.toLowerCase() ?? '';
    if (content.includes('meeting') || content.includes('sync')) return 'meeting_room';
    if (content.includes('idea') || content.includes('brainstorm')) return 'lightbulb';
    if (content.includes('todo') || content.includes('task')) return 'format_list_bulleted';
    return 'article';
  }

  getRelativeTime(note: INote): string {
    const updated = note.updatedAt ? new Date(note.updatedAt) : (note.createdAt ? new Date(note.createdAt) : new Date());
    const now = new Date();
    const diffMs = now.getTime() - updated.getTime();
    const diffHrs = Math.floor(diffMs / (1000 * 60 * 60));
    const diffDays = Math.floor(diffHrs / 24);

    if (diffHrs < 1) return 'Just now';
    if (diffHrs < 24) return `${diffHrs} hour${diffHrs > 1 ? 's' : ''} ago`;
    if (diffDays === 1) return 'Yesterday';
    if (diffDays < 7) return `${diffDays} days ago`;

    const opts: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric' };
    return updated.toLocaleDateString('en-US', opts);
  }

  toggleTimer() {
    if (this.timerRunning()) {
      this.timerRunning.set(false);
      if (this.timerInterval) clearInterval(this.timerInterval);
    } else {
      this.timerRunning.set(true);
      this.timerInterval = setInterval(() => {
        if (this.timerSeconds() > 0) {
          this.timerSeconds.update(s => s - 1);
        } else {
          this.timerRunning.set(false);
          if (this.timerInterval) clearInterval(this.timerInterval);
        }
      }, 1000);
    }
  }

  resetTimer() {
    this.timerRunning.set(false);
    if (this.timerInterval) clearInterval(this.timerInterval);
    this.timerSeconds.set(24 * 60 + 59);
  }
}
