import { Component, ElementRef, Input, Output, EventEmitter, ViewChild, ChangeDetectionStrategy } from '@angular/core';

import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './search-bar.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./search-bar.component.scss']
})
export class SearchBarComponent {
  @Input() query = '';
  @Input() placeholder = 'Search...';
  @Input() shortcutKey = ''; 
  
  @Output() queryChange = new EventEmitter<string>();

  @ViewChild('searchInput') searchInputRef!: ElementRef<HTMLInputElement>;

  onQueryChange(newQuery: string) {
    this.query = newQuery;
    this.queryChange.emit(this.query);
  }

  clearSearch() {
    this.query = '';
    this.queryChange.emit(this.query);
    this.focus();
  }

  focus() {
    if (this.searchInputRef) {
      this.searchInputRef.nativeElement.focus();
    }
  }

  blur() {
    if (this.searchInputRef) {
      this.searchInputRef.nativeElement.blur();
    }
  }
  
  get isFocused(): boolean {
    if (!this.searchInputRef) return false;
    return document.activeElement === this.searchInputRef.nativeElement;
  }
}
