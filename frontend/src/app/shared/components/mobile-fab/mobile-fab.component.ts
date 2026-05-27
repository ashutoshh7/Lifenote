import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-mobile-fab',
  standalone: true,
  templateUrl: './mobile-fab.component.html',
  styleUrls: ['./mobile-fab.component.scss']
})
export class MobileFabComponent {
  @Input() icon = 'add';
  @Input() title = '';
  @Output() fabClick = new EventEmitter<Event>();

  onClick(event: Event) {
    this.fabClick.emit(event);
  }
}
