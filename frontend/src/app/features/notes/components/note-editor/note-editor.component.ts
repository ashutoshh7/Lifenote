import {
  Component,
  input,
  output,
  signal,
  computed,
  ViewChild,
  ElementRef,
  ChangeDetectionStrategy,
  effect,
  untracked,
  OnDestroy,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { INote } from '../../../../core/models/note.model';
import { MarkdownPreviewComponent } from '../markdown-preview/markdown-preview.component';
import { formatMarkdownText } from '../../utils/markdown-formatter';

@Component({
  selector: 'app-note-editor',
  standalone: true,
  imports: [FormsModule, MarkdownPreviewComponent],
  templateUrl: './note-editor.component.html',
  styleUrls: ['./note-editor.component.scss'],
  changeDetection: ChangeDetectionStrategy.Eager
})
export class NoteEditorComponent implements OnInit, OnDestroy {
  // Inputs & Outputs
  note = input<INote | null>(null);
  saveStatus = input<'idle' | 'saving' | 'saved'>('idle');
  isMobileOrTablet = input<boolean>(false);

  save = output<{ id: string | null; title: string; content: string; tags: string[] }>();
  close = output<void>();

  // Editor form local signals
  editorTitle = signal('');
  editorContent = signal('');
  editorTags = signal<string[]>([]);
  tagInput = signal('');
  isPreviewMode = signal(false);
  isFullscreen = signal(false);

  @ViewChild('contentTextarea') contentTextareaRef!: ElementRef<HTMLTextAreaElement>;

  private currentNoteId: string | null = null;
  private autoSaveSubject = new Subject<void>();
  private autoSaveSub = this.autoSaveSubject.pipe(debounceTime(1000)).subscribe(() => {
    this.triggerSaveIfModified();
  });

  // Track if changes have been made relative to the current input note
  isModified = computed(() => {
    const active = this.note();
    const currentTitle = this.editorTitle();
    const currentContent = this.editorContent();
    const currentTags = this.editorTags();

    if (!active) {
      return currentTitle.trim() !== '' || currentContent.trim() !== '' || currentTags.length > 0;
    }

    const originalTitle = active.title || '';
    const originalContent = active.content || '';
    const originalTags = active.tags || [];

    const tagsMatch = originalTags.length === currentTags.length &&
      originalTags.every((t, i) => t === currentTags[i]);

    return currentTitle !== originalTitle ||
      currentContent !== originalContent ||
      !tagsMatch;
  });

  constructor() {
    // Synchronize editor state when the active note changes
    effect(() => {
      const active = this.note();

      untracked(() => {
        // Save current note before switching if it was modified
        if (this.isModified()) {
          this.save.emit({
            id: this.currentNoteId,
            title: this.editorTitle(),
            content: this.editorContent(),
            tags: this.editorTags()
          });
        }

        // Update current note ID
        this.currentNoteId = active ? active.id : null;

        // Load new note
        if (active) {
          this.editorTitle.set(active.title || '');
          this.editorContent.set(active.content || '');
          this.editorTags.set([...(active.tags || [])]);
        } else {
          this.editorTitle.set('');
          this.editorContent.set('');
          this.editorTags.set([]);
        }
        this.tagInput.set('');
        this.isPreviewMode.set(false);
      });
    });
  }

  ngOnInit() {}

  ngOnDestroy() {
    // Save any pending changes on component destruction
    if (this.isModified()) {
      this.save.emit({
        id: this.currentNoteId,
        title: this.editorTitle(),
        content: this.editorContent(),
        tags: this.editorTags()
      });
    }
    this.autoSaveSub.unsubscribe();
    this.autoSaveSubject.complete();
  }

  triggerChange() {
    this.autoSaveSubject.next();
  }

  private triggerSaveIfModified() {
    if (this.isModified()) {
      this.save.emit({
        id: this.currentNoteId,
        title: this.editorTitle(),
        content: this.editorContent(),
        tags: this.editorTags()
      });
    }
  }

  getSaveStatusTitle(): string {
    switch (this.saveStatus()) {
      case 'saving': return 'Saving changes...';
      case 'saved': return 'All changes saved to cloud';
      default: return 'Draft';
    }
  }

  addTag() {
    const val = this.tagInput().trim();
    if (val && !this.editorTags().includes(val)) {
      this.editorTags.update(tags => [...tags, val]);
      this.triggerChange();
    }
    this.tagInput.set('');
  }

  removeTag(tag: string) {
    this.editorTags.update(tags => tags.filter(t => t !== tag));
    this.triggerChange();
  }

  togglePreview() {
    this.isPreviewMode.update(v => !v);
  }

  toggleFullscreen() {
    this.isFullscreen.update(v => !v);
  }

  format(type: 'bold' | 'italic' | 'ul' | 'ol') {
    const el = this.contentTextareaRef?.nativeElement;
    if (!el) return;

    const start = el.selectionStart;
    const end = el.selectionEnd;
    const content = this.editorContent();

    const { newContent, newCursorStart, newCursorEnd } = formatMarkdownText(
      content,
      start,
      end,
      type
    );

    this.editorContent.set(newContent);
    this.triggerChange();

    setTimeout(() => {
      el.setSelectionRange(newCursorStart, newCursorEnd);
      el.focus();
    }, 0);
  }

  closeEditor() {
    this.close.emit();
  }
}
