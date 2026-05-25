import { Component, inject, signal, computed, OnInit, effect } from '@angular/core';
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
  
  // Editor form
  editorTitle = signal('');
  editorContent = signal('');
  isPreviewMode = signal(false);

  // Computed
  notes = computed(() => this.notesService.notes());
  isMobile = computed(() => this.breakpointService.isMobile());
  hasNotes = computed(() => this.notes().length > 0);

  filteredNotes = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.notes();

    return this.notes().filter(note =>
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

  ngOnInit() {
    this.notesService.getAllNotes().subscribe((notes) => {
      if (notes.length > 0 && !this.activeNoteId() && !this.isMobile()) {
        this.selectNote(notes[0]);
      }
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
  }

  triggerAutoSave() {
    this.autoSaveSubject.next();
  }

  private autoSave() {
    const title = this.editorTitle().trim() || 'Untitled';
    const content = this.editorContent();

    if (!title && !content.trim()) return;

    if (this.activeNoteId()) {
      const active = this.activeNote();
      const noteData = { title, content, isPinned: active?.isPinned ?? false };
      this.notesService.updateNote(this.activeNoteId()!, noteData).subscribe();
    } else {
      const noteData: ICreateNoteDto = { title, content };
      this.notesService.createNote(noteData).subscribe(newNote => {
        this.activeNoteId.set(newNote.id);
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
        }
      });
    }
  }

  togglePin(id: number, event: Event) {
    event.stopPropagation();
    this.notesService.togglePin(id).subscribe();
  }

  getNotePreview(content: string): string {
    if (!content) return 'No additional text';
    return content.substring(0, 60) + (content.length > 60 ? '...' : '');
  }

  closeEditorOnMobile() {
    this.activeNoteId.set(null);
  }

  setViewMode(mode: 'list' | 'grid') {
    this.viewMode.set(mode);
  }
}
