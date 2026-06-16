import { Component, inject, signal, computed, OnInit, ChangeDetectionStrategy, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { INote, ICreateNoteDto } from '../../../../core/models/note.model';
import { NotesService } from './note-page-services/notes.service';
import { BreakpointService } from '../../../../core/services/breakpoint.service';
import { SearchBarComponent, MobileFabComponent, SkeletonLoaderComponent } from '../../../../shared';
import { ToastService } from '../../../../core/services/toast.service';
import { NoteEditorComponent } from '../../components/note-editor/note-editor.component';

@Component({
  selector: 'app-notes-page',
  standalone: true,
  imports: [
    FormsModule,
    SearchBarComponent,
    MobileFabComponent,
    SkeletonLoaderComponent,
    NoteEditorComponent
  ],
  templateUrl: './notes-page.component.html',
  styleUrls: ['./notes-page.component.scss'],
  changeDetection: ChangeDetectionStrategy.Eager
})
export class NotesPageComponent implements OnInit {
  private notesService = inject(NotesService);
  private breakpointService = inject(BreakpointService);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  // Search and View State
  searchQuery = signal('');
  activeNoteId = signal<string | null>(null);
  viewMode = signal<'list' | 'grid'>('list');
  showArchived = signal(false);
  saveStatus = signal<'idle' | 'saving' | 'saved'>('idle');
  isEditing = signal(false);

  // Computed Properties
  isLoading = computed(() => this.notesService.isLoading());
  notes = computed(() => this.notesService.notes());
  isMobileOrTablet = computed(() => !this.breakpointService.isDesktop());
  hasNotes = computed(() => this.notes().length > 0);

  filteredNotes = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const showArchivedVal = this.showArchived();

    const baseNotes = this.notes().filter(note => !!note.isArchived === showArchivedVal);
    if (!query) return baseNotes;

    return baseNotes.filter(note =>
      note.title.toLowerCase().includes(query) ||
      note.content.toLowerCase().includes(query) ||
      note.tags?.some(tag => tag.toLowerCase().includes(query))
    );
  });

  pinnedNotes = computed(() => this.filteredNotes().filter(n => n.isPinned));
  regularNotes = computed(() => this.filteredNotes().filter(n => !n.isPinned));

  activeNote = computed(() => this.notes().find(n => n.id === this.activeNoteId()) ?? null);

  constructor() {
    effect(() => {
      if (!this.isLoading() && this.notes().length > 0) {
        const idParam = this.route.snapshot.queryParamMap.get('id');
        const actionParam = this.route.snapshot.queryParamMap.get('action');

        if (idParam && !this.activeNoteId()) {
          const targetNote = this.notes().find(n => n.id === idParam);
          if (targetNote) {
            this.selectNote(targetNote);
            const queryParams: any = { id: null };
            if (this.isMobileOrTablet()) queryParams.editing = 'true';
            this.router.navigate([], { queryParams, queryParamsHandling: 'merge', replaceUrl: true });
          }
        } else if (actionParam === 'new' && !this.isEditing()) {
          this.openNewNote();
          const queryParams: any = { action: null };
          if (this.isMobileOrTablet()) queryParams.editing = 'true';
          this.router.navigate([], { queryParams, queryParamsHandling: 'merge', replaceUrl: true });
        }
      }
    });
  }

  ngOnInit() {
    // Handle native back button on mobile
    this.route.queryParams.subscribe(params => {
      if (!params['editing'] && this.isEditing() && this.isMobileOrTablet()) {
        this.isEditing.set(false);
        this.activeNoteId.set(null);
      }
    });
  }

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
  }

  openNewNote() {
    this.activeNoteId.set(null);
    this.isEditing.set(true);
    this.saveStatus.set('idle');

    if (this.isMobileOrTablet()) {
      this.router.navigate([], { queryParams: { editing: 'true' }, queryParamsHandling: 'merge' });
    }
  }

  selectNote(note: INote) {
    if (this.activeNoteId() === note.id) return;

    this.activeNoteId.set(note.id);
    this.isEditing.set(true);
    this.saveStatus.set('saved');

    if (this.isMobileOrTablet()) {
      this.router.navigate([], { queryParams: { editing: 'true' }, queryParamsHandling: 'merge' });
    }
  }

  saveNote(event: { id: string | null; title: string; content: string; tags: string[] }) {
    const title = event.title.trim() || 'Untitled';
    const content = event.content;
    const tags = event.tags;

    // Do not save if title is empty and content is empty
    if (!event.title.trim() && !content.trim()) return;

    this.saveStatus.set('saving');

    if (event.id) {
      const targetNote = this.notes().find(n => n.id === event.id);
      const noteData = {
        title,
        content,
        isPinned: targetNote?.isPinned ?? false,
        tags
      };
      this.notesService.updateNote(event.id, noteData).subscribe({
        next: () => {
          // If the note being saved is still the active note, set saveStatus to saved
          if (this.activeNoteId() === event.id) {
            this.saveStatus.set('saved');
          }
        },
        error: () => this.saveStatus.set('idle')
      });
    } else {
      const noteData: ICreateNoteDto = { title, content, tags };
      this.notesService.createNote(noteData).subscribe({
        next: (newNote) => {
          // If we were editing a new note (activeNoteId is null), link it to the newly created note
          if (this.activeNoteId() === null) {
            this.activeNoteId.set(newNote.id);
          }
          this.saveStatus.set('saved');
        },
        error: () => this.saveStatus.set('idle')
      });
    }
  }

  deleteNote(id: string, event: Event) {
    event.stopPropagation();
    if (confirm('Delete this note?')) {
      this.notesService.deleteNote(id).subscribe({
        next: () => {
          this.toastService.show('Note deleted successfully.', 'success');
          if (this.activeNoteId() === id) {
            this.activeNoteId.set(null);
            this.isEditing.set(false);
          }
        },
        error: () => this.toastService.show('Failed to delete note.', 'error')
      });
    }
  }

  togglePin(id: string, event: Event) {
    event.stopPropagation();
    this.notesService.togglePin(id).subscribe();
  }

  toggleArchive(id: string, event: Event) {
    event.stopPropagation();
    this.notesService.toggleArchive(id).subscribe({
      next: () => {
        this.toastService.show('Note archive status updated.', 'success');
        if (this.activeNoteId() === id) {
          this.activeNoteId.set(null);
          this.isEditing.set(false);
        }
      },
      error: () => this.toastService.show('Failed to update note status.', 'error')
    });
  }

  toggleShowArchived() {
    this.showArchived.update(v => !v);
    this.activeNoteId.set(null);
    this.isEditing.set(false);
  }

  getNotePreview(content: string): string {
    if (!content) return 'No additional text';
    return content.substring(0, 60) + (content.length > 60 ? '...' : '');
  }

  closeEditorOnMobile() {
    this.activeNoteId.set(null);
    this.isEditing.set(false);

    // Clean up query param if closing manually (e.g. from UI back button)
    this.router.navigate([], { queryParams: { editing: null }, queryParamsHandling: 'merge' });
  }

  setViewMode(mode: 'list' | 'grid') {
    this.viewMode.set(mode);
  }
}

