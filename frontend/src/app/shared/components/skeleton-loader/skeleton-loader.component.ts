import { Component, Input, ChangeDetectionStrategy } from '@angular/core';


export type SkeletonVariant = 'notes-list' | 'goals-grid' | 'dashboard' | 'note-items';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [],
  templateUrl: './skeleton-loader.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./skeleton-loader.component.scss']
})
export class SkeletonLoaderComponent {
  /** Which skeleton layout to render */
  @Input() variant: SkeletonVariant = 'notes-list';

  /** How many repeating skeleton rows/cards to show */
  @Input() count = 4;

  get items(): number[] {
    return Array.from({ length: this.count }, (_, i) => i);
  }
}
