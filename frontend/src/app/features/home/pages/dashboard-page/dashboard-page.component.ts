import { Component, inject, computed, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { NotesService } from '../../../notes/pages/notes-page/note-page-services/notes.service';
import { GoalService } from '../../../goals/services/goal.service';
import { PomodoroService } from '../../../pomodoro/services/pomodoro.service';
import { AuthService } from '../../../../core/services/auth.service';
import { INote } from '../../../../core/models/note.model';
import { IGoal } from '../../../goals/models/goal.model';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  private notesService = inject(NotesService);
  private goalService = inject(GoalService);
  private pomodoroService = inject(PomodoroService);
  authService = inject(AuthService);
  private router = inject(Router);

  recentNotes = computed(() => this.notesService.notes().slice(0, 4));
  totalNotes = computed(() => this.notesService.notes().length);

  activeGoals = computed(() => this.goalService.activeGoals().slice(0, 2));
  totalActiveGoals = computed(() => this.goalService.activeGoals().length);

  focusHoursToday = signal<number>(0);
  currentStreak = signal<number>(0);

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

  ngOnInit() {
    this.notesService.getAllNotes().subscribe();
    this.goalService.getAllGoals().subscribe();
    this.pomodoroService.getFocusStats().subscribe({
      next: (res: any) => {
        if (res && res.data) {
          this.focusHoursToday.set(res.data.todayFocusHours || 0);
          this.currentStreak.set(res.data.currentStreak || 0);
        }
      },
      error: (err: any) => console.error('Failed to load focus stats', err)
    });
  }

  navigateTo(path: string) {
    this.router.navigate([path]);
  }

  createNewNote() {
    this.router.navigate(['/notes'], { queryParams: { action: 'new' } });
  }

  createNewGoal() {
    this.router.navigate(['/goals', 'new']);
  }

  navigateToNote(noteId: number) {
    this.router.navigate(['/notes'], { queryParams: { id: noteId } });
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

  getGoalProgress(goal: IGoal): number {
    return this.goalService.getProgress(goal);
  }

  getCompletedMilestonesCount(goal: IGoal): number {
    return goal.milestones.filter(m => m.isCompleted).length;
  }
}
