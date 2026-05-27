import { Component, inject, signal, computed, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { INote, ICreateNoteDto } from '../../../../core/models/note.model';
import { NotesService } from './note-page-services/notes.service';
import { BreakpointService } from '../../../../core/services/breakpoint.service';
import { Subject, debounceTime } from 'rxjs';
import { MarkdownPreviewComponent } from '../../components/markdown-preview/markdown-preview.component';
import { SearchBarComponent, MobileFabComponent } from '../../../../shared';

@Component({
  selector: 'app-notes-page',
  standalone: true,
  imports: [CommonModule, FormsModule, MarkdownPreviewComponent, SearchBarComponent, MobileFabComponent],
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
  editorTags = signal<string[]>([]);
  tagInput = signal('');
  isPreviewMode = signal(false);
  isEditing = signal(false);
  isFullscreen = signal(false);

  @ViewChild('contentTextarea') contentTextareaRef!: ElementRef<HTMLTextAreaElement>;

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
      const actionParam = this.route.snapshot.queryParamMap.get('action');
      
      if (idParam) {
        const id = Number(idParam);
        const targetNote = notes.find(n => n.id === id);
        if (targetNote) {
          this.selectNote(targetNote);
          // Clear query param so refreshes don't re-select it incorrectly
          this.router.navigate([], { queryParams: { id: null }, queryParamsHandling: 'merge', replaceUrl: true });
          return;
        }
      } else if (actionParam === 'new') {
        this.openNewNote();
        this.router.navigate([], { queryParams: { action: null }, queryParamsHandling: 'merge', replaceUrl: true });
        return;
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
    this.editorTags.set([]);
    this.tagInput.set('');
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
    this.editorTags.set([...(note.tags || [])]);
    this.tagInput.set('');
    this.isPreviewMode.set(false);
    this.isEditing.set(true);
  }

  triggerAutoSave() {
    this.autoSaveSubject.next();
  }

  private autoSave() {
    const title = this.editorTitle().trim() || 'Untitled';
    const content = this.editorContent();
    const tags = this.editorTags();

    if (!this.editorTitle().trim() && !content.trim()) return;

    if (this.activeNoteId()) {
      const active = this.activeNote();
      const noteData = { title, content, isPinned: active?.isPinned ?? false, tags };
      this.notesService.updateNote(this.activeNoteId()!, noteData).subscribe();
    } else {
      const noteData: ICreateNoteDto = { title, content, tags };
      this.notesService.createNote(noteData).subscribe(newNote => {
        if (this.activeNoteId() === null) {
          this.activeNoteId.set(newNote.id);
        }
      });
    }
  }

  addTag() {
    const val = this.tagInput().trim();
    if (val && !this.editorTags().includes(val)) {
      this.editorTags.update(tags => [...tags, val]);
      this.triggerAutoSave();
    }
    this.tagInput.set('');
  }

  removeTag(tag: string) {
    this.editorTags.update(tags => tags.filter(t => t !== tag));
    this.triggerAutoSave();
  }

  togglePreview() {
    this.isPreviewMode.update(v => !v);
  }

  toggleFullscreen() {
    this.isFullscreen.update(v => !v);
  }

  /**
   * Applies markdown syntax wrapping or prefix to the selected text in the textarea.
   * @param type - 'bold' | 'italic' | 'ul' | 'ol'
   */
  format(type: 'bold' | 'italic' | 'ul' | 'ol') {
    const el = this.contentTextareaRef?.nativeElement;
    if (!el) return;

    const start = el.selectionStart;
    const end = el.selectionEnd;
    const content = this.editorContent();
    const selected = content.substring(start, end);

    let newContent = content;
    let newCursorStart = start;
    let newCursorEnd = end;

    if (type === 'bold') {
      const wrapped = `**${selected || 'bold text'}**`;
      newContent = content.substring(0, start) + wrapped + content.substring(end);
      newCursorStart = start + 2;
      newCursorEnd = start + 2 + (selected || 'bold text').length;
    } else if (type === 'italic') {
      const wrapped = `*${selected || 'italic text'}*`;
      newContent = content.substring(0, start) + wrapped + content.substring(end);
      newCursorStart = start + 1;
      newCursorEnd = start + 1 + (selected || 'italic text').length;
    } else if (type === 'ul' || type === 'ol') {
      // Split selected lines and prefix each
      const lines = selected
        ? selected.split('\n').map((line, i) =>
            type === 'ul' ? `- ${line}` : `${i + 1}. ${line}`
          ).join('\n')
        : type === 'ul' ? '- List item' : '1. List item';

      // Ensure there's a newline before the list if not at start of line
      const before = content.substring(0, start);
      const prefix = before.length > 0 && !before.endsWith('\n') ? '\n' : '';
      newContent = before + prefix + lines + '\n' + content.substring(end);
      newCursorStart = start + prefix.length;
      newCursorEnd = newCursorStart + lines.length;
    }

    this.editorContent.set(newContent);
    this.triggerAutoSave();

    // Restore cursor/selection after Angular updates the DOM
    setTimeout(() => {
      el.setSelectionRange(newCursorStart, newCursorEnd);
      el.focus();
    }, 0);
  }

  deleteNote(id: number, event: Event) {
    event.stopPropagation();
    if (confirm('Delete this note?')) {
      this.notesService.deleteNote(id).subscribe(() => {
        if (this.activeNoteId() === id) {
          this.activeNoteId.set(null);
          this.editorTitle.set('');
          this.editorContent.set('');
          this.editorTags.set([]);
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
        this.editorTags.set([]);
        this.isEditing.set(false);
      }
    });
  }

  toggleShowArchived() {
    this.showArchived.update(v => !v);
    this.activeNoteId.set(null);
    this.editorTitle.set('');
    this.editorContent.set('');
    this.editorTags.set([]);
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
