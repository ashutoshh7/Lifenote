
import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';
import { LucideAngularModule, Pin, Trash2, Edit } from 'lucide-angular';
import { INote } from '../../../../core/models/note.model';

@Component({
  selector: 'app-note-card',
  standalone: true,
  imports: [LucideAngularModule],
  templateUrl: './note-card.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './note-card.component.scss',
})
export class NoteCardComponent {
  note = input.required<INote>();
  clicked = output<INote>();
  edit = output<INote>();
  delete = output<number>();
  togglePin = output<number>();

  // Icons
  PinIcon = Pin;
  TrashIcon = Trash2;
  EditIcon = Edit;

  getContentPreview(): string {
    const content = this.note().content || '';
    return content.length > 100 ? content.substring(0, 100) + '...' : content;
  }

  formatDate(date?: Date): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  }

  onCardClick() {
    this.clicked.emit(this.note());
  }

  onEdit(event: Event) {
    event.stopPropagation();
    this.edit.emit(this.note());
  }

  onTogglePin(event: Event) {
    event.stopPropagation();
    this.togglePin.emit(this.note().id);
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.delete.emit(this.note().id);
  }
}
