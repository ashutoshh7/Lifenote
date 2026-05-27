import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty-state" [class.large]="variant === 'large'">
      @if (icon) {
        <span class="material-symbols-outlined empty-icon">{{ icon }}</span>
      }
      <ng-content></ng-content>
    </div>
  `,
  styleUrls: ['./empty-state.component.scss']
})
export class EmptyStateComponent {
  @Input() icon?: string;
  @Input() variant: 'default' | 'large' = 'default';
}
