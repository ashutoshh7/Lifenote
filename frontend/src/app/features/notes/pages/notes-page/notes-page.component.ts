import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { INote, ICreateNoteDto } from '../../../../core/models/note.model';
import { NotesService } from './note-page-services/notes.service';
import { BreakpointService } from '../../../../core/services/breakpoint.service';
import { Subject, debounceTime } from 'rxjs';
import { MarkdownPreviewComponent } from '../../components/markdown-preview/markdown-preview.component';

@Component({
  selector: 'app-notes-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownPreviewComponent],
  templateUrl: './notes-page.component.html',
  styleUrls: ['./notes-page.component.scss']
})
export class NotesPageComponent implements OnInit {
  private notesService = inject(NotesService);
  private breakpointService = inject(BreakpointService);
  private autoSaveSubject = new Subject<void>();

  // Signals
  searchQuery = signal('');
  activeNoteId = signal<number | null>(null);
  viewMode = signal<'list' | 'grid'>('list');
  showArchived = signal(false);

  // Editor form
  editorTitle = signal('');
  editorContent = signal('');
  isPreviewMode = signal(false);
  isEditing = signal(false);

  // Computed
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
    this.autoSaveSubject.pipe(debounceTime(1000)).subscribe(() => {
      this.autoSave();
    });
  }

  private router = inject(Router);
  private route = inject(ActivatedRoute);

  ngOnInit() {
    this.notesService.getAllNotes().subscribe((notes) => {
      const idParam = this.route.snapshot.queryParamMap.get('id');
      if (idParam) {
        const id = Number(idParam);
        const targetNote = notes.find(n => n.id === id);
        if (targetNote) {
          this.selectNote(targetNote);
          // Clear query param so refreshes don't re-select it incorrectly
          this.router.navigate([], { queryParams: { id: null }, queryParamsHandling: 'merge', replaceUrl: true });
          return;
        }
      }
      // Auto-selection of the first note is intentionally removed
    });
  }

  onSearch(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
  }

  openNewNote() {
    this.activeNoteId.set(null);
    this.editorTitle.set('');
    this.editorContent.set('');
    this.isPreviewMode.set(false);
    this.isEditing.set(true);
  }

  selectNote(note: INote) {
    if (this.activeNoteId() === note.id) return;

    // Save current before switching
    if (this.editorTitle().trim() || this.editorContent().trim()) {
      this.autoSave();
    }

    this.activeNoteId.set(note.id);
    this.editorTitle.set(note.title);
    this.editorContent.set(note.content);
    this.isPreviewMode.set(false);
    this.isEditing.set(true);
  }

  triggerAutoSave() {
    this.autoSaveSubject.next();
  }

  private autoSave() {
    const title = this.editorTitle().trim() || 'Untitled';
    const content = this.editorContent();

    if (!this.editorTitle().trim() && !content.trim()) return;

    if (this.activeNoteId()) {
      const active = this.activeNote();
      const noteData = { title, content, isPinned: active?.isPinned ?? false };
      this.notesService.updateNote(this.activeNoteId()!, noteData).subscribe();
    } else {
      const noteData: ICreateNoteDto = { title, content };
      this.notesService.createNote(noteData).subscribe(newNote => {
        if (this.activeNoteId() === null) {
          this.activeNoteId.set(newNote.id);
        }
      });
    }
  }

  togglePreview() {
    this.isPreviewMode.update(v => !v);
  }

  deleteNote(id: number, event: Event) {
    event.stopPropagation();
    if (confirm('Delete this note?')) {
      this.notesService.deleteNote(id).subscribe(() => {
        if (this.activeNoteId() === id) {
          this.activeNoteId.set(null);
          this.editorTitle.set('');
          this.editorContent.set('');
          this.isEditing.set(false);
        }
      });
    }
  }

  togglePin(id: number, event: Event) {
    event.stopPropagation();
    this.notesService.togglePin(id).subscribe();
  }

  toggleArchive(id: number, event: Event) {
    event.stopPropagation();
    this.notesService.toggleArchive(id).subscribe(() => {
      if (this.activeNoteId() === id) {
        this.activeNoteId.set(null);
        this.editorTitle.set('');
        this.editorContent.set('');
        this.isEditing.set(false);
      }
    });
  }

  toggleShowArchived() {
    this.showArchived.update(v => !v);
    this.activeNoteId.set(null);
    this.editorTitle.set('');
    this.editorContent.set('');
    this.isEditing.set(false);
  }

  getNotePreview(content: string): string {
    if (!content) return 'No additional text';
    return content.substring(0, 60) + (content.length > 60 ? '...' : '');
  }

  closeEditorOnMobile() {
    this.activeNoteId.set(null);
    this.isEditing.set(false);
  }

  setViewMode(mode: 'list' | 'grid') {
    this.viewMode.set(mode);
  }
}
