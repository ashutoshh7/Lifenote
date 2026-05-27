import { Component, inject, signal, computed, OnInit, HostListener, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { GoalService } from '../../services/goal.service';
import { IGoal, GoalCategory, GoalStatus } from '../../models/goal.model';

type SortOption = 'newest' | 'oldest' | 'due-soon' | 'progress-asc' | 'progress-desc' | 'a-z';

import { SearchBarComponent, MobileFabComponent } from '../../../../shared';
import { GoalCardComponent } from '../../../../shared/components/goal-card/goal-card.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-goals-page',
  standalone: true,
  imports: [CommonModule, FormsModule, SearchBarComponent, MobileFabComponent, GoalCardComponent, EmptyStateComponent],
  templateUrl: './goals-page.component.html',
  styleUrls: ['./goals-page.component.scss']
})
export class GoalsPageComponent implements OnInit {
  private goalService = inject(GoalService);
  private router = inject(Router);

  @ViewChild('searchBar') searchBar!: SearchBarComponent;

  Math = Math;

  // ---- Filter & sort state ----
  showMobileFilters = signal(false);
  searchQuery = signal('');
  activeStatusFilter = signal<GoalStatus | 'All'>('All');
  activeCategoryFilter = signal<GoalCategory | 'All'>('All');
  sortBy = signal<SortOption>('newest');

  readonly statusFilters: (GoalStatus | 'All')[] = ['All', 'Active', 'Paused', 'Completed', 'Archived'];
  readonly categories: (GoalCategory | 'All')[] = ['All', 'Work', 'Personal', 'Health', 'Finance', 'Learning', 'Other'];
  readonly sortOptions: { value: SortOption; label: string }[] = [
    { value: 'newest',       label: 'Newest First' },
    { value: 'oldest',       label: 'Oldest First' },
    { value: 'due-soon',     label: 'Due Soon' },
    { value: 'progress-asc', label: 'Progress ↑' },
    { value: 'progress-desc',label: 'Progress ↓' },
    { value: 'a-z',          label: 'A – Z' },
  ];

  // ---- Source + derived ----
  allGoals = computed(() => this.goalService.goals());

  filteredGoals = computed(() => {
    let list = this.allGoals();

    // Search
    const q = this.searchQuery().trim().toLowerCase();
    if (q) {
      list = list.filter(g =>
        g.title.toLowerCase().includes(q) ||
        g.category?.toLowerCase().includes(q) ||
        g.description?.toLowerCase().includes(q)
      );
    }

    // Status filter
    const sf = this.activeStatusFilter();
    if (sf !== 'All') list = list.filter(g => g.status === sf);

    // Category filter
    const cf = this.activeCategoryFilter();
    if (cf !== 'All') list = list.filter(g => g.category === cf);

    // Sort
    const sort = this.sortBy();
    return [...list].sort((a, b) => {
      switch (sort) {
        case 'newest':       return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
        case 'oldest':       return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
        case 'due-soon': {
          const aT = a.targetDate ? new Date(a.targetDate).getTime() : Infinity;
          const bT = b.targetDate ? new Date(b.targetDate).getTime() : Infinity;
          return aT - bT;
        }
        case 'progress-asc': return this.goalService.getProgress(a) - this.goalService.getProgress(b);
        case 'progress-desc':return this.goalService.getProgress(b) - this.goalService.getProgress(a);
        case 'a-z':          return a.title.localeCompare(b.title);
        default:             return 0;
      }
    });
  });

  goals         = computed(() => this.goalService.goals());
  activeGoals   = computed(() => this.goalService.activeGoals());
  completedGoals= computed(() => this.goalService.completedGoals());

  hasActiveFilters = computed(() =>
    this.searchQuery().trim() !== '' ||
    this.activeStatusFilter() !== 'All' ||
    this.activeCategoryFilter() !== 'All' ||
    this.sortBy() !== 'newest'
  );

  ngOnInit(): void {
    this.goalService.getAllGoals().subscribe();
  }

  // ---- Actions ----
  openGoal(id: number)  { this.router.navigate(['/goals', id]); }
  newGoal()             { this.router.navigate(['/goals', 'new']); }
  toggleMobileFilters() { this.showMobileFilters.update(v => !v); }

  clearFilters() {
    this.searchQuery.set('');
    this.activeStatusFilter.set('All');
    this.activeCategoryFilter.set('All');
    this.sortBy.set('newest');
  }

  // ---- Keyboard shortcuts ----
  @HostListener('document:keydown', ['$event'])
  onKeydown(e: KeyboardEvent) {
    const tag = (e.target as HTMLElement).tagName.toLowerCase();
    const isTyping = tag === 'input' || tag === 'textarea' || tag === 'select';

    // N → new goal (only when not typing)
    if (e.key === 'n' && !isTyping && !e.metaKey && !e.ctrlKey) {
      e.preventDefault();
      this.newGoal();
    }

    // / → focus search (only when not already typing)
    if (e.key === '/' && !isTyping) {
      e.preventDefault();
      this.searchBar?.focus();
    }

    // Escape → clear search if focused
    if (e.key === 'Escape' && this.searchBar?.isFocused) {
      this.searchQuery.set('');
      this.searchBar.blur();
    }
  }


}
