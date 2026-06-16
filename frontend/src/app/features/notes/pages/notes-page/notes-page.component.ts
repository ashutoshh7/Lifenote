import { Component, inject, signal, computed, OnInit, ViewChild, ElementRef, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { FormsModule } from '@angular/forms';
import { INote, ICreateNoteDto } from '../../../../core/models/note.model';
import { NotesService } from './note-page-services/notes.service';
import { BreakpointService } from '../../../../core/services/breakpoint.service';
import { Subject, debounceTime } from 'rxjs';
import { MarkdownPreviewComponent } from '../../components/markdown-preview/markdown-preview.component';
import { SearchBarComponent, MobileFabComponent, SkeletonLoaderComponent } from '../../../../shared';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-notes-page',
  standalone: true,
  imports: [FormsModule, MarkdownPreviewComponent, SearchBarComponent, MobileFabComponent, SkeletonLoaderComponent],
  templateUrl: './notes-page.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./notes-page.component.scss']
})
  }

  openNewNote() {
    this.activeNoteId.set(null);
    this.editorTitle.set('');
    this.editorContent.set('');
    this.editorTags.set([]);
    this.tagInput.set('');
    this.isPreviewMode.set(false);
    this.isEditing.set(true);
    this.saveStatus.set('idle');
    
    if (this.isMobileOrTablet()) {
      this.router.navigate([], { queryParams: { editing: 'true' }, queryParamsHandling: 'merge' });
    }
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
    this.saveStatus.set('saved');
    
    if (this.isMobileOrTablet()) {
      this.router.navigate([], { queryParams: { editing: 'true' }, queryParamsHandling: 'merge' });
    }
  }

  triggerAutoSave() {
    this.saveStatus.set('saving');
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
      this.notesService.updateNote(this.activeNoteId()!, noteData).subscribe({
        next: () => this.saveStatus.set('saved'),
        error: () => this.saveStatus.set('idle')
      });
    } else {
      const noteData: ICreateNoteDto = { title, content, tags };
      this.notesService.createNote(noteData).subscribe({
        next: (newNote) => {
          if (this.activeNoteId() === null) {
            this.activeNoteId.set(newNote.id);
          }
          this.saveStatus.set('saved');
        },
        error: () => this.saveStatus.set('idle')
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

  deleteNote(id: string, event: Event) {
    event.stopPropagation();
    if (confirm('Delete this note?')) {
      this.notesService.deleteNote(id).subscribe({
        next: () => {
          this.toastService.show('Note deleted successfully.', 'success');
          if (this.activeNoteId() === id) {
            this.activeNoteId.set(null);
            this.editorTitle.set('');
            this.editorContent.set('');
            this.editorTags.set([]);
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
          this.editorTitle.set('');
          this.editorContent.set('');
          this.editorTags.set([]);
          this.isEditing.set(false);
        }
      },
      error: () => this.toastService.show('Failed to update note status.', 'error')
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
    
    // Clean up query param if closing manually (e.g. from UI back button)
    this.router.navigate([], { queryParams: { editing: null }, queryParamsHandling: 'merge' });
  }

  setViewMode(mode: 'list' | 'grid') {
    this.viewMode.set(mode);
  }
}
